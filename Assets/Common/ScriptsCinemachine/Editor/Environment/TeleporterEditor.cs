using UnityEditor;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     Custom editor for Teleporters that draws their name in scene view
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Teleporter), true)]
    [InitializeOnLoad]
    public class TeleporterEditor : Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawHandles(Teleporter teleporter, GizmoType gizmoType)
        {
            var t = teleporter;

            var style = new GUIStyle();

            // draws the path item number
            style.normal.textColor = Color.cyan;
            style.alignment = TextAnchor.UpperCenter;
            var verticalOffset = t.transform.lossyScale.x > 0 ? 1f : 2f;
            Handles.Label(t.transform.position + Vector3.up * (2f + verticalOffset), t.name, style);
        }
    }
}