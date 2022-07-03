using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MoreMountains.Tools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MMTrailRendererSortingLayer))]
    public class MMTrailRendererLayerEditor : Editor
    {
        protected MMTrailRendererSortingLayer _mmTrailRendererSortingLayer;
        protected TrailRenderer _trailRenderer;
        private int popupMenuIndex;
        private string[] sortingLayerNames;

        private void OnEnable()
        {
            sortingLayerNames = GetSortingLayerNames();
            _mmTrailRendererSortingLayer = (MMTrailRendererSortingLayer)target;
            _trailRenderer = _mmTrailRendererSortingLayer.GetComponent<TrailRenderer>();

            for (var i = 0;
                 i < sortingLayerNames.Length;
                 i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
                if (sortingLayerNames[i] == _trailRenderer.sortingLayerName)
                    popupMenuIndex = i;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_trailRenderer == null) return;

            popupMenuIndex = EditorGUILayout.Popup("Sorting Layer", popupMenuIndex, sortingLayerNames);
            var newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", _trailRenderer.sortingOrder);

            if (sortingLayerNames[popupMenuIndex] != _trailRenderer.sortingLayerName
                || newSortingLayerOrder != _trailRenderer.sortingOrder)
            {
                Undo.RecordObject(_trailRenderer, "Change Particle System Renderer Order");

                _trailRenderer.sortingLayerName = sortingLayerNames[popupMenuIndex];
                _trailRenderer.sortingOrder = newSortingLayerOrder;

                EditorUtility.SetDirty(_trailRenderer);
            }
        }

        public string[] GetSortingLayerNames()
        {
            var internalEditorUtilityType = typeof(InternalEditorUtility);
            var sortingLayersProperty =
                internalEditorUtilityType.GetProperty("sortingLayerNames",
                    BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
    }
}