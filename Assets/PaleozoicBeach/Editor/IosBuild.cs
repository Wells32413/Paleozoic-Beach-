using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PaleozoicBeach.Editor
{
    public static class IosBuild
    {
        [MenuItem("Paleozoic Beach/Build iPhone Xcode project")]
        public static void Build()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, BuildTarget.iOS))
                throw new InvalidOperationException("Install this Editor's iOS Build Support module first.");
            PlayerSettings.companyName = "Paleozoic Beach";
            PlayerSettings.productName = "Paleozoic Beach";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.iOS.buildNumber = Environment.GetEnvironmentVariable("PB_BUILD_NUMBER") ?? "1";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS,
                Environment.GetEnvironmentVariable("PB_BUNDLE_ID") ?? "com.paleozoicbeach.game");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
            PlayerSettings.iOS.targetOSVersionString = "15.0";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.runInBackground = false;
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            string team = Environment.GetEnvironmentVariable("PB_APPLE_TEAM_ID");
            if (!string.IsNullOrEmpty(team)) PlayerSettings.iOS.appleDeveloperTeamID = team;
            // Touch uses UI events; desktop keys require the legacy input backend as well.
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var input = settings.FindProperty("activeInputHandler");
            if (input != null && input.intValue != 0)
                throw new InvalidOperationException("Set Active Input Handling to Input Manager (Old), restart Unity, then build again.");
            string output = Environment.GetEnvironmentVariable("PB_IOS_OUTPUT") ?? "Builds/iOS";
            Directory.CreateDirectory(output);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/PaleozoicBeach/Scenes/Beach.unity" },
                locationPathName = output, target = BuildTarget.iOS, options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException("iPhone export failed: " + report.summary.result);
            Debug.Log("Xcode project exported to " + Path.GetFullPath(output) + ". Signing and device verification still required.");
        }
    }
}
