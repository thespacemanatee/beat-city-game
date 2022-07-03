using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     Custom editor for Rooms that draws their name in scene view
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Room), true)]
    [InitializeOnLoad]
    public class RoomEditor : Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawHandles(Room room, GizmoType gizmoType)
        {
            var t = room;

            var style = new GUIStyle();

            // draws the path item number
            style.normal.textColor = MMColors.Pink;
            Handles.Label(t.transform.position + Vector3.up * 2f + Vector3.right * 2f, t.name, style);
        }
    }
}