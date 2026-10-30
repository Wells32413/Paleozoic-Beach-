using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace PaleozoicBeach.Editor
{
    public static class ProjectSetup
    {
        [MenuItem("Paleozoic Beach/Create playable scene")]
        public static void CreateScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            System.IO.Directory.CreateDirectory("Assets/PaleozoicBeach/Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/PaleozoicBeach/Scenes/Beach.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scene.path, true) };
            PlayerSettings.companyName = "Paleozoic Beach";
            PlayerSettings.productName = "Paleozoic Beach";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS, "com.paleozoicbeach.game");
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.paleozoicbeach.game");
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            AssetDatabase.SaveAssets();
        }
    }
}
