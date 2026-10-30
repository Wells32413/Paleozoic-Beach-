# Paleozoic Beach

Unity mobile game prototype: explore a Pennsylvanian Illinois estuary, document Bandringa, return after eight minutes, analyze research, fund educational exhibits, and finance another expedition.

## Open and play

1. Install Unity **6.3 LTS**, starting with project pin **6000.3.0f1** (use the latest supported 6000.3 patch for production after validation). The Editor/version and package availability were not verified in this environment. Add iOS Build Support and Android Build Support as needed.
2. Open this directory in Unity Hub. Allow package import. The project uses the built-in render pipeline; URP is a future migration.
3. Open the included `Assets/PaleozoicBeach/Scenes/Beach.unity` scene, or run **Paleozoic Beach → Create playable scene** to regenerate it and apply mobile settings. This saves `Assets/PaleozoicBeach/Scenes/Beach.unity`, registers it in Build Settings, and applies mobile identifiers/orientation. This command replaces the open scene; save other work first.
4. Set **Active Input Handling** to **Input Manager (Old)** or **Both**, then press Play. The bootstrap generates all runtime geometry and UI. No asset purchase is needed.
5. Enter the facility and launch. Use W/S forward/reverse, A/D yaw, Q/E ascend/descend, or on-screen hold controls. Look up/down with the on-screen buttons. Space or Photograph takes a research observation; point the center crosshair at Bandringa within 35 metres with a clear line of sight.
6. Survive the full **480 seconds**. There is no accelerated timer in the shipped slice. Return is automatic. Open the park, build a $300 research exhibit, operate park days, and launch again for $250.

The park is a functional simplified management interface. The facility is an interface, not a modeled building. Photography records observation quality and counts; it does **not** yet save camera image files. There is no video recording in this milestone.

The prototype starts with $1,000. The introductory expedition is grant-funded until a successful return. Each newly discovered species pays a one-time $200 research grant. Exhibits unlock per discovered species; open days produce $150 revenue minus $50 expenses per exhibit. Pause freezes the mission. Backgrounding pauses it; tap Pause to resume. Mid-expedition quitting loses unsent observations and any paid launch fee; expedition resumption is not implemented. Keep a save backup when upgrading.

## Architecture

- `Assets/PaleozoicBeach/Core`: plain C#, no Unity references. Species eligibility, expedition resources/timer, observation quality, research settlement, and park economy.
- `Assets/PaleozoicBeach/Runtime`: Unity adapter, generated environment, camera/submarine movement, wildlife presentation, mobile controls, UI and JSON persistence.
- `Assets/PaleozoicBeach/Resources/species.json`: editable catalog with age, habitat, eligibility and uncertainty. Only Bandringa is enabled. Disabled ranges are provisional metadata, not approved spawn rules.
- `Assets/PaleozoicBeach/Editor`: repeatable playable scene setup.
- `Assets/PaleozoicBeach/Tests/EditMode`: NUnit simulation tests.
- `docs`: scientific basis, roadmap, and verification handoff.

Runtime state progression: Menu → Facility → Dive → Results → Park/Facility. One owner ticks an expedition only while active and unpaused. Settlement is idempotent per mission; only a surviving return merges discoveries. Rendering is separate from the Core assembly. Future content should extend a validated region/time/habitat catalog rather than bypass eligibility.

Saves are versioned JSON in `Application.persistentDataPath/research.json`, with a previous-save `.bak` and temp-file staging. Loading tries the backup if the main save is invalid. Writes currently delete then rename the main file, leaving a recoverable crash window; this is not a transactional storage guarantee. Saves are local and unencrypted. Currency is prototype integer dollars.

## Tests and builds

Run **Window → General → Test Runner → EditMode → Run All**. For automation after generating the scene:

```sh
Unity -batchmode -nographics -projectPath "$PWD" -runTests -testPlatform EditMode -testResults TestResults/editmode.xml -logFile TestResults/editor.log
```

Run `python3 tools/check_project.py` for repository consistency checks. These do not compile C# or substitute for Unity tests. See [verification status](docs/VERIFICATION.md) before making build claims. GitHub Actions includes a runnable consistency job; Unity CI requires licensed runners and is documented rather than falsely represented as passing.

For iOS, switch Build Profiles to iOS, build Xcode output, then sign and run from a compatible macOS/Xcode host. Provisioning, team identifier, minimum OS and device tiers must be chosen and validated. For Android, select IL2CPP/ARM64, set API levels, create signing credentials outside version control, and test an AAB. No platform build has been executed here.

No third-party art, sound, textures or fonts are bundled. Built-in Unity primitives, Standard shader, and LegacyRuntime font are used. Photorealistic art, natural rigged animation, detailed cockpit, audio, particles, water surface and scalable quality presets are still required. See [roadmap](docs/ROADMAP.md).
