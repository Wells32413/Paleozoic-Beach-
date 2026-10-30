# Verification status and handoff

## Environment evidence

This work was authored in a Linux workspace without Unity, dotnet, csc or mcs. An HTTPS request to the official .NET installer failed because the configured proxy at proxy:8080 was unreachable. No C# compilation, NUnit execution, Unity import, rendering test, platform build or device test was possible. Source consistency checks passed (26 assertions/parses); these are not C# tests.

## Implemented in source, pending Editor verification

- Main menu and present-day research facility UI.
- Runtime-generated 3D estuary with sediment and rocks.
- First-person submarine and visible cockpit frame; move/yaw/depth/pitch controls.
- Bandringa placeholder with bounded, smooth swimming path, no attacks.
- Eight-minute timer, battery/oxygen depletion, hull damage on obstructions/bounds, scheduled return or failed expedition.
- Species/date/habitat gate; photos require range, alignment and line of sight. Quality and counts are stored, not image files.
- Results screen; research analysis and grants; repeated settlement protected.
- Versioned local saves with backup fallback, park exhibits and funding persistence.
- Simplified exhibit purchasing and open-day income that finances later expeditions.
- Eight NUnit tests authored covering core timing, validity, grants, failure, idempotence and economy.

## Required Editor checks

1. Generate playable scene, import cleanly, verify no Console errors and run all EditMode tests.
2. Enter menu/facility; launch grant-funded expedition. Verify camera is outside hull, controls respond and initial animal is findable.
3. Take a valid photo, then try out-of-frame, distant and rock-obscured photos. Ensure invalid photos do not add research. Repeated photos improve best quality and count.
4. Complete an actual eight-minute mission. Timer never advances in pause/background; UI resumes correctly. No accidental hull depletion at idle in open water.
5. Verify failed mission discards research. Test terrain/obstruction response and bounds. Movement uses linecasts, not full-volume collision, so grazing/clipping requires further work.
6. Return, analyze, build exhibit, operate park and pay for another expedition. No duplicate discovery grant. Confirm insufficient-fund handling.
7. Restart application; funding, exhibits and research survive. Corrupt main save and verify backup fallback. Simulate unwritable storage and inspect error reporting.
8. Test touches on multiple controls, pointer release outside buttons, interrupted app lifecycle, aspect ratios and safe areas. Current fixed canvas is a prototype, not production-responsive UI.
9. Build iOS via macOS/Xcode and Android via Unity. Record real device profiler results. The source has not been exercised on either.

## Known limitations

No finished visual assets, sophisticated ecology, video, scanning, objective selection, equipment/upgrades, 3D park placement, visitors/staff, audio or quality presets. Facility and park are functional interfaces. No active-mission resume; quitting discards observations and keeps paid expedition costs. No production save migration or simultaneous save access. Unproductive paid missions can exhaust funding before an exhibit exists. No precise location/stratigraphy or photorealistic/anatomically approved models. Realistic graphics are a project direction, not an achieved milestone.

Target repository: https://github.com/Wells32413/Paleozoic-Beach- . Publication is performed via the connected GitHub tools because this environment’s CLI authentication is unavailable.
