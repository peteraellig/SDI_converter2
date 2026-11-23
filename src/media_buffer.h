#pragma once
#include <algorithm>
#include <array>
#include <cmath>
#include <cstdint>
#include <deque>
#include <mutex>
#include <vector>
#include <stdexcept>
// `start` is the chunk's first sample on a continuous stream sample counter (set by MediaBuffer on push).
struct VideoPicture { int64_t timestamp=0; std::vector<uint8_t> pixels; };
struct Sound { int64_t timestamp=0; int samples=0; std::vector<float> planar; int64_t start=0; };
inline int32_t pcm32(float x) {
    if(!std::isfinite(x))return 0;
    const double v=std::clamp(static_cast<double>(x),-1.0,1.0);
    return v>=1 ? INT32_MAX : static_cast<int32_t>(v*2147483648.0);
}

// Windowed-sinc interpolator for reading audio at fractional positions. The playout reads the source a tiny bit
// faster or slower (at most 0.1 %) to follow the clock difference between sender and SDI card without gaps or jumps.
// At an integer position it returns the source samples unchanged (cut-off at Nyquist, sinc zeros on the taps).
class Interpolator {
public:
    static constexpr int half=16, taps=2*half, phases=512;
    static const Interpolator& instance(){ static const Interpolator value; return value; }
    // Taps for src[i0-half+1 .. i0+half] at fraction `frac` (0..1) after i0.
    void weights(double frac,std::array<float,taps>& out) const {
        const double p=frac*phases; const int a=std::clamp(static_cast<int>(p),0,phases-1); const float f=static_cast<float>(p-a);
        const auto& lo=table[a]; const auto& hi=table[a+1];
        for(int t=0;t<taps;++t)out[t]=lo[t]+(hi[t]-lo[t])*f;
    }
private:
    std::vector<std::array<float,taps>> table;
    Interpolator():table(phases+1){
        constexpr double pi=3.14159265358979323846;
        for(int p=0;p<=phases;++p){
            const double frac=static_cast<double>(p)/phases; double sum=0; std::array<double,taps> h{};
            for(int t=0;t<taps;++t){
                const double x=(t-half+1)-frac;
                const double sinc=std::abs(x)<1e-12?1.0:std::sin(pi*x)/(pi*x);
                const double w=0.42+0.5*std::cos(pi*x/half)+0.08*std::cos(2*pi*x/half); // Blackman over +-half
                h[t]=std::abs(x)>=half?0.0:sinc*w; sum+=h[t];
            }
            for(int t=0;t<taps;++t)table[p][t]=static_cast<float>(h[t]/sum);
        }
    }
};

class MediaBuffer {
    std::mutex mutex;
    std::deque<VideoPicture> video;
    std::deque<Sound> audio;
    int64_t previous=-1;
    uint64_t generation=0,discarded=0;
    // Stream sample counter: sample 0 is at `audioBase` (100 ns). A chunk whose timestamp says it follows the previous one
    // within 2 samples is placed exactly after it, so rounding in the sender's timestamps never makes a gap or an overlap.
    bool haveBase=false; int64_t audioBase=0;
    static int64_t samplesFor(int64_t ticks){ return static_cast<int64_t>(std::llround(static_cast<double>(ticks)*48000.0/10000000.0)); }
public:
    struct State {size_t count;int64_t first,last;uint64_t generation,discarded;};
    void push(VideoPicture picture) {
        std::lock_guard lock(mutex);
        if(previous>=0 && (picture.timestamp<previous || picture.timestamp-previous>10000000)){
            video.clear();audio.clear();haveBase=false;++generation;
        }
        if(picture.timestamp==previous)return;
        previous=picture.timestamp;
        if(video.size()>=12){video.pop_front();++discarded;}
        video.push_back(std::move(picture));
    }
    void push(Sound sound) {
        std::lock_guard lock(mutex);
        if(!audio.empty() && sound.timestamp<audio.back().timestamp){audio.clear();haveBase=false;}
        while(!audio.empty() && (audio.size()>=64 || sound.timestamp-audio.front().timestamp>5000000))audio.pop_front();
        if(!haveBase){audioBase=sound.timestamp;haveBase=true;}
        const int64_t computed=samplesFor(sound.timestamp-audioBase);
        if(!audio.empty()){
            const int64_t end=audio.back().start+audio.back().samples;
            sound.start=std::llabs(computed-end)<=2 ? end : computed;
        } else sound.start=computed;
        audio.push_back(std::move(sound));
    }
    State state() { std::lock_guard lock(mutex);
        return {video.size(),video.empty()?0:video.front().timestamp,previous,generation,discarded}; }
    bool picture(int64_t target,int64_t tolerance,VideoPicture& selected) {
        std::lock_guard lock(mutex);bool found=false;
        while(!video.empty() && video.front().timestamp<=target+tolerance){
            if(found)++discarded;
            selected=std::move(video.front());video.pop_front();found=true;
        }return found;
    }
    // `count` stereo samples for the SDI output, read from source time origin + offsetTicks onward with `ratio` source samples
    // per output sample. OMT is planar float; DeckLink expects interleaved signed PCM32 at 48 kHz. Missing source = silence.
    std::vector<int32_t> sound(int64_t origin,double offsetTicks,double ratio,int count,uint64_t& missing) {
        std::lock_guard lock(mutex);std::vector<int32_t> result(static_cast<size_t>(count)*2,0);
        if(!haveBase||count<=0){missing+=static_cast<uint64_t>(std::max(count,0));return result;}
        constexpr int half=Interpolator::half;
        const double start=(static_cast<double>(origin-audioBase)+offsetTicks)*48000.0/10000000.0;
        const int64_t first=static_cast<int64_t>(std::floor(start))-half+1;
        const int64_t last=static_cast<int64_t>(std::floor(start+(count-1)*ratio))+half;
        const size_t span=static_cast<size_t>(last-first+1);
        while(!audio.empty() && audio.front().start+audio.front().samples<=first)audio.pop_front();
        std::vector<float> left(span,0.0f),right(span,0.0f); std::vector<uint8_t> present(span,0);
        for(const auto& chunk:audio){
            if(chunk.start>last)break;
            for(int i=0;i<chunk.samples;++i){
                const int64_t at=chunk.start+i-first;
                if(at<0)continue;
                if(at>=static_cast<int64_t>(span))break;
                left[static_cast<size_t>(at)]=chunk.planar[i]; right[static_cast<size_t>(at)]=chunk.planar[chunk.samples+i]; present[static_cast<size_t>(at)]=1;
            }
        }
        const auto& interpolator=Interpolator::instance(); std::array<float,Interpolator::taps> w{};
        for(int k=0;k<count;++k){
            const double pos=start+k*ratio-static_cast<double>(first);
            // Rounding guard: keep all taps inside the fetched span.
            const int64_t i0=std::clamp(static_cast<int64_t>(std::floor(pos)),static_cast<int64_t>(half-1),static_cast<int64_t>(span)-half-1);
            interpolator.weights(std::clamp(pos-static_cast<double>(i0),0.0,1.0),w);
            float l=0,r=0; const size_t base=static_cast<size_t>(i0-half+1);
            for(int t=0;t<Interpolator::taps;++t){l+=left[base+t]*w[t];r+=right[base+t]*w[t];}
            if(!present[static_cast<size_t>(i0)])++missing;
            result[static_cast<size_t>(k)*2]=pcm32(l); result[static_cast<size_t>(k)*2+1]=pcm32(r);
        }
        return result;
    }
};
