using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Custom editor for the MMScreenSafeZones component
    /// </summary>
    [CustomEditor(typeof(MMAspectRatioSafeZones), true)]
    [CanEditMultipleObjects]
    public class MMAspectRatioSafeZonesEditor : Editor
    {
        private static MMAspectRatioSafeZones safeZones;

        /// <summary>
        ///     On enable, registers to the OnSceneGUI hook
        /// </summary>
        private void OnEnable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            safeZones = (MMAspectRatioSafeZones)target;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        ///     OnSceneGUI, draws center and ratios
        /// </summary>
        /// <param name="sceneView"></param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            DrawFrameCenter(sceneView);
            DrawRatios(sceneView);
        }

        /// <summary>
        ///     Draws a rectangle for each ratio
        /// </summary>
        /// <param name="sceneView"></param>
        private static void DrawRatios(SceneView sceneView)
        {
            if (!safeZones.DrawRatios) return;

            var center = sceneView.pivot;

            var width = sceneView.position.width;
            var height = sceneView.position.height;

            var bottomLeft = new Vector3(center.x - width / 2f, center.y - height / 2f, 0f);
            var topRight = new Vector3(center.x + width / 2f, center.y + height / 2f, 0f);

            var topLeft = bottomLeft;
            topLeft.y = topRight.y;
            var bottomRight = topRight;
            bottomRight.y = bottomLeft.y;

            var size = safeZones.CameraSize;

            // dotted lines
            var spacing = 2f;
            var dottedLineColor = Color.white;
            dottedLineColor.a = 0.4f;
            Handles.color = dottedLineColor;
            // top
            Handles.DrawDottedLine(new Vector3(topLeft.x, center.y + size, 0f),
                new Vector3(topRight.x, center.y + size, 0f), spacing);
            // bottom
            Handles.DrawDottedLine(new Vector3(topLeft.x, center.y - size, 0f),
                new Vector3(topRight.x, center.y - size, 0f), spacing);

            foreach (var ratio in safeZones.Ratios)
                if (ratio.DrawRatio)
                {
                    var aspectRatio = ratio.Size.x / ratio.Size.y;

                    Handles.color = ratio.RatioColor;

                    // aspect ratio positions
                    var ratioTopLeft = new Vector3(center.x - size * aspectRatio, center.y + size, 0f);
                    var ratioTopRight = new Vector3(center.x + size * aspectRatio, center.y + size, 0f);
                    var ratioBottomLeft = new Vector3(center.x - size * aspectRatio, center.y - size, 0f);
                    var ratioBottomRight = new Vector3(center.x + size * aspectRatio, center.y - size, 0f);
                    var ratioLabelPosition = ratioBottomLeft + 0.1f * Vector3.down + 0.1f * Vector3.right;

                    // draws a label under the rectangle
                    var style = new GUIStyle();
                    style.normal.textColor = ratio.RatioColor;
                    style.fontSize = 8;
                    Handles.Label(ratioLabelPosition, ratio.Size.x + ":" + ratio.Size.y, style);

                    // draws a rectangle around the aspect ratio
                    Vector3[] verts = { ratioTopLeft, ratioTopRight, ratioBottomRight, ratioBottomLeft };
                    Handles.DrawSolidRectangleWithOutline(verts, new Color(0, 0, 0, 0), ratio.RatioColor);

                    // draws the dead zone of that ratio
                    var zoneColor = ratio.RatioColor;
                    zoneColor.a = zoneColor.a * safeZones.UnsafeZonesOpacity;

                    // top rectangle
                    verts = new[]
                    {
                        topLeft, topRight, new(topLeft.x, ratioTopLeft.y, 0f),
                        new Vector3(topRight.x, ratioTopRight.y, 0f)
                    };
                    Handles.DrawSolidRectangleWithOutline(verts, zoneColor, new Color(0, 0, 0, 0));

                    // bottom rectangle
                    verts = new[]
                    {
                        bottomLeft, new(topLeft.x, ratioBottomLeft.y, 0f),
                        new Vector3(topRight.x, ratioBottomRight.y, 0f), bottomRight
                    };
                    Handles.DrawSolidRectangleWithOutline(verts, zoneColor, new Color(0, 0, 0, 0));

                    // left rectangle
                    verts = new[]
                    {
                        new(topLeft.x, ratioTopLeft.y, 0f), ratioTopLeft, ratioBottomLeft,
                        new Vector3(bottomLeft.x, ratioBottomLeft.y, 0f)
                    };
                    Handles.DrawSolidRectangleWithOutline(verts, zoneColor, new Color(0, 0, 0, 0));

                    // right rectangle
                    verts = new[]
                    {
                        new(topRight.x, ratioTopRight.y, 0f), new Vector3(bottomRight.x, ratioBottomRight.y, 0f),
                        ratioBottomRight, ratioTopRight
                    };
                    Handles.DrawSolidRectangleWithOutline(verts, zoneColor, new Color(0, 0, 0, 0));

                    // dotted line left 
                    Handles.DrawDottedLine(new Vector3(ratioBottomLeft.x, topLeft.y, 0f),
                        new Vector3(ratioTopLeft.x, bottomLeft.y, 0f), spacing);
                    // dotted line right 
                    Handles.DrawDottedLine(new Vector3(ratioBottomRight.x, topLeft.y, 0f),
                        new Vector3(ratioBottomRight.x, bottomLeft.y, 0f), spacing);
                }
        }

        /// <summary>
        ///     Draws a crosshair at the center
        /// </summary>
        /// <param name="sceneView"></param>
        private static void DrawFrameCenter(SceneView sceneView)
        {
            if (!safeZones.DrawCenterCrosshair) return;

            var center = sceneView.pivot;
            var crossHairSize = safeZones.CenterCrosshairSize;

            var reticleSize = crossHairSize / 10f;

            Handles.color = safeZones.CenterCrosshairColor;

            var crosshairTopLeft = new Vector3(center.x - crossHairSize / 2f, center.y + crossHairSize / 2f, 0f);
            var crosshairTopRight = new Vector3(center.x + crossHairSize / 2f, center.y + crossHairSize / 2f, 0f);
            var crosshairBottomLeft = new Vector3(center.x - crossHairSize / 2f, center.y - crossHairSize / 2f, 0f);
            var crosshairBottomRight = new Vector3(center.x + crossHairSize / 2f, center.y - crossHairSize / 2f, 0f);

            // cross
            Handles.DrawLine(new Vector3(center.x, center.y + crossHairSize / 2f, 0f),
                new Vector3(center.x, center.y - crossHairSize / 2f, 0f));
            Handles.DrawLine(new Vector3(center.x - crossHairSize / 2f, center.y, 0f),
                new Vector3(center.x + crossHairSize / 2f, center.y, 0f));

            // top left
            Handles.DrawLine(crosshairTopLeft, new Vector3(crosshairTopLeft.x + reticleSize, crosshairTopLeft.y, 0f));
            Handles.DrawLine(crosshairTopLeft, new Vector3(crosshairTopLeft.x, crosshairTopLeft.y - reticleSize, 0f));
            // top right
            Handles.DrawLine(crosshairTopRight,
                new Vector3(crosshairTopRight.x - reticleSize, crosshairTopRight.y, 0f));
            Handles.DrawLine(crosshairTopRight,
                new Vector3(crosshairTopRight.x, crosshairTopRight.y - reticleSize, 0f));
            // bottom left
            Handles.DrawLine(crosshairBottomLeft,
                new Vector3(crosshairBottomLeft.x + reticleSize, crosshairBottomLeft.y, 0f));
            Handles.DrawLine(crosshairBottomLeft,
                new Vector3(crosshairBottomLeft.x, crosshairBottomLeft.y + reticleSize, 0f));
            // bottom right
            Handles.DrawLine(crosshairBottomRight,
                new Vector3(crosshairBottomRight.x - reticleSize, crosshairBottomRight.y, 0f));
            Handles.DrawLine(crosshairBottomRight,
                new Vector3(crosshairBottomRight.x, crosshairBottomRight.y + reticleSize, 0f));
        }
    }
}