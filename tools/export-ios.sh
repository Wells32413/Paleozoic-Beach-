#!/usr/bin/env bash
set -euo pipefail
if [[ "$(uname -s)" != Darwin ]]; then
  echo 'iPhone export requires a Mac with Unity iOS Build Support and Xcode.' >&2
  exit 2
fi
if [[ -z "${UNITY_EDITOR:-}" || ! -x "$UNITY_EDITOR" ]]; then
  echo 'Set UNITY_EDITOR to the executable inside your Unity.app.' >&2
  exit 2
fi
xcodebuild -version
project_dir="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$project_dir/Builds/Logs" "$project_dir/TestResults"
"$UNITY_EDITOR" -batchmode -nographics -projectPath "$project_dir" \
  -runTests -testPlatform EditMode \
  -testResults "$project_dir/TestResults/editmode.xml" \
  -logFile "$project_dir/Builds/Logs/tests.log"
python3 "$project_dir/tools/require_unity_tests.py" "$project_dir/TestResults/editmode.xml"
"$UNITY_EDITOR" -batchmode -quit -projectPath "$project_dir" -buildTarget iOS \
  -executeMethod PaleozoicBeach.Editor.IosBuild.Build \
  -logFile "$project_dir/Builds/Logs/ios-export.log"
echo 'Xcode export completed. Open Builds/iOS/Unity-iPhone.xcodeproj, choose your signing team and connected iPhone, then Run.'
echo 'This export is not a signed IPA and does not establish device compatibility.'
