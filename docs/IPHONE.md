# Getting the native prototype onto an iPhone

## Current delivery status

This repository contains Unity source, not an installable iPhone app. No IPA or Xcode export has been generated. The development workspace is Linux without Unity or Xcode, and no Apple signing identity or build service is connected. Source changes for touch input, safe-area fitting and a bundled mobile shader have not been run on a device.

The project owner currently has neither a Mac nor an Apple Developer membership. That blocks native delivery with the tools available in this session. Source code cannot be installed directly from GitHub onto an iPhone.

## Delivery choices

- **Personal device testing:** access to a Mac with compatible Xcode, Unity 6.3 LTS plus iOS Build Support, an iPhone and an Apple Account. Xcode's free Personal Team may be sufficient for local testing, subject to Apple's provisioning limits and expiry. Paid membership is not inherently required for this route.
- **TestFlight delivery:** an Apple Developer Program membership, an App Store Connect app and signing setup, and a Mac build machine or a configured cloud build service. TestFlight can then supply an installation link after successful builds and any required beta review. No service has been purchased or configured here.
- **Browser alternative:** a separate Safari-compatible playable version could avoid native signing. No browser port has been created, and this is not an installable Unity app.

Do not send Apple passwords, private keys or signing certificates in chat. Configure signing on the build machine or in the chosen service's secret storage.

## Reproducible Mac export

1. Clone this repository and open it in the pinned Unity LTS Editor. Install iOS Build Support and a compatible Xcode. Use Input Manager (Old) and restart if Unity prompts.
2. Run the EditMode suite in Test Runner. Inspect the scene and Console, then run the eight-minute loop in Play Mode before exporting.
3. In Unity choose **Paleozoic Beach → Build iPhone Xcode project**. This sets the device SDK, iPhone-only target, landscape orientation, IL2CPP, prototype iOS 15.0 minimum, version 0.1.0 and automatic signing. The minimum OS is a prototype setting, not a verified compatibility promise.
4. Alternatively, from a Mac terminal:

```sh
export UNITY_EDITOR='/Applications/Unity/Hub/Editor/6000.3.0f1/Unity.app/Contents/MacOS/Unity'
# Optional non-secret configuration: PB_APPLE_TEAM_ID, PB_BUNDLE_ID, PB_BUILD_NUMBER.
# Choose a bundle identifier you control; the default is a placeholder.
bash tools/export-ios.sh
```

The script gates export on a nonempty successful EditMode report. It produces `Builds/iOS` and local test/build logs; it does not sign or upload the app. The Editor menu export does not automatically run tests.

5. Open `Builds/iOS/Unity-iPhone.xcodeproj`. In Signing & Capabilities choose your Apple team and an available bundle identifier. Select the connected iPhone and Run. Follow Apple's Developer Mode and provisioning instructions if prompted.
6. Verify simultaneous touches, notch/Dynamic Island safe areas, legible text, camera framing, pause/resume, heat/frame rate, the full expedition and persisted research after relaunch. Current UI fitting is source-only and still needs physical target-size testing.
7. For TestFlight, archive and validate with the appropriate distribution signing, then upload through Xcode/App Store Connect. Required app metadata, icons, privacy declarations and review must be completed before wider release.

Do not describe an Xcode export as an installed or tested game. Record the actual Editor/Xcode versions, iPhone model, OS, build number, logs and results when these steps are performed.
