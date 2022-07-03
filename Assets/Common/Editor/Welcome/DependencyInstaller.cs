using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.TopDownEngine
{
    public class DependencyInstaller : Editor
    {
        public static void InstallDependencies()
        {
            var installHappened = false;

            if (!PackageInstallation.IsInstalled(WelcomeWindow.PostProcessingPackageID))
            {
                InstallPostProcessing();
                installHappened = true;
            }

            if (!PackageInstallation.IsInstalled(WelcomeWindow.CinemachinePackageID))
            {
                InstallCinemachine();
                installHappened = true;
            }

            if (!PackageInstallation.IsInstalled(WelcomeWindow.PixelPerfectPackageID))
            {
                InstallPixelPerfect();
                installHappened = true;
            }

            if (installHappened)
            {
                AssetDatabase.Refresh();
                ReloadCurrentScene();
            }
        }

        public static void ReloadCurrentScene()
        {
            var currentScenePath = SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(currentScenePath);
        }

        public static void InstallPostProcessing()
        {
            if (!PackageInstallation.IsInstalled(WelcomeWindow.PostProcessingPackageID))
                PackageInstallation.Install(WelcomeWindow.PostProcessingPackageVersionID);
        }

        public static void InstallCinemachine()
        {
            if (!PackageInstallation.IsInstalled(WelcomeWindow.CinemachinePackageID))
                PackageInstallation.Install(WelcomeWindow.CinemachinePackageVersionID);
        }

        public static void InstallPixelPerfect()
        {
            if (!PackageInstallation.IsInstalled(WelcomeWindow.PixelPerfectPackageID))
                PackageInstallation.Install(WelcomeWindow.PixelPerfectPackageVersionID);
        }

        public class AfterImport : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                string[] movedAssets, string[] movedFromAssetPaths)
            {
                foreach (var importedAsset in importedAssets)
                    if (importedAsset.Contains("DependencyInstaller.cs"))
                    {
                        //WelcomeWindow.ShowWindow(); 
                    }
            }
        }

        public class DefineSymbol : Editor
        {
            private static bool ObsoleteBuild(BuildTargetGroup group)
            {
                var attributes = typeof(BuildTargetGroup).GetField(group.ToString())
                    .GetCustomAttributes(typeof(ObsoleteAttribute), false);
                return attributes.Length > 0 && attributes != null;
            }
        }
    }

    public class PackageInstallation
    {
        public static bool IsInstalled(string packageID)
        {
            var packagesFolder = Application.dataPath + "/../Packages/";
            var manifestFile = packagesFolder + "manifest.json";
            var manifest = File.ReadAllText(manifestFile);

            return manifest.Contains(packageID);
        }

        public static void Install(string packageVersionID)
        {
            Client.Add(packageVersionID);
        }
    }
}