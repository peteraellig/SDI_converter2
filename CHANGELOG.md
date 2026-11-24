# Changelog

## 2.0.0.2 (2026-12-23)
Latencies measured and optimized: 
## SDI → OMT: 
about 1 frame, or roughly 20 ms. It can’t be any shorter, because the card doesn’t output a frame until it has been received in its entirety. Compression with VMX and transmission take only a few milliseconds.
## OMT → SDI:
Over SDI, a frame arrives line by line, spread out over the entire frame duration. At 50p, that’s 20 ms. The card writes the lines to memory and doesn’t output the frame until the last line is there. Blackmagic’s programming interface only delivers complete frames.
The card transmits continuously at its own clock rate. Once a frame has finished transmitting, the next one must already be fully available; otherwise, there will be a black screen or a repeated frame. Added to this is the buffer margin, since the transmitter and the SDI card cannot operate at the same clock rate. Connecting the card to genlock does not change this.

Previously approx. 80 ms
New buffer settings (test them yourself)
Buffer 3 = ~60 ms
Buffer 2 = ~40 ms


## 2.0.0.1 (2026-11-23)

- **NDI audio level.** Each channel has a new audio-level checkbox. By default, the audio level stays unchanged. Selecting the checkbox applies the NDI reference level (+20 dB). Both settings preserve the audio level on a round trip between SDI and NDI.
- **Self-healing and startup.**
  - An output waits for its source for as long as necessary.
  - Automatic restart is enabled by default. If an engine fails, it restarts after 3 seconds. Repeated failures increase the delay up to a maximum of 30 seconds.
  - Select the startup checkbox on a channel to start it automatically when the program opens.
- **Clock differences.** The output smoothly adjusts its playback rate to follow the source clock and resamples the audio, avoiding pops and frame skips caused by clock drift.
- **SD video.** PAL and NTSC are reported with a 4:3 aspect ratio and the BT.601 color space.
- **Frame rate.** Minor differences in frame-rate notation, such as 5994/100 instead of 60000/1001, no longer cause the program to crash.
- **Format changes.** A single corrupted frame no longer triggers a restart with a black screen. The output switches only after it receives 10 consecutive frames in the new format.
- **NDI with an alpha channel.** The converter now accepts these sources, converts the video to UYVY and discards the alpha channel.
