using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    [CustomEditor(typeof(MMAspectRatioSafeZones), true)]
    public class MMScreenshotEditor : Editor
    {
        private static readonly string FolderName = "Screenshots";

        [MenuItem("Tools/More Mountains/Screenshot/Take Screenshot Real Size", false, 801)]
        public static void MenuScreenshotSize1()
        {
            var savePath = TakeScreenCaptureScreenshot(1);
        }

        [MenuItem("Tools/More Mountains/Screenshot/Take Screenshot Size x2", false, 802)]
        public static void MenuScreenshotSize2()
        {
            var savePath = TakeScreenCaptureScreenshot(2);
        }

        [MenuItem("Tools/More Mountains/Screenshot/Take Screenshot Size x3 %k", false, 803)]
        public static void MenuScreenshotSize3()
        {
            var savePath = TakeScreenCaptureScreenshot(3);
        }

        protected static string TakeScreenCaptureScreenshot(int gameViewSizeMultiplier)
        {
            if (!Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName);

            float width = Screen.width * gameViewSizeMultiplier;
            float height = Screen.height * gameViewSizeMultiplier;
            var savePath = FolderName + "/screenshot_" + width + "x" + height + "_" +
                           DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

            ScreenCapture.CaptureScreenshot(savePath, gameViewSizeMultiplier);
            Debug.Log("[MMScreenshot] Screenshot taken with size multiplier of " + gameViewSizeMultiplier +
                      " and saved at " + savePath);
            return savePath;
        }
    }
}