# Changelog

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
