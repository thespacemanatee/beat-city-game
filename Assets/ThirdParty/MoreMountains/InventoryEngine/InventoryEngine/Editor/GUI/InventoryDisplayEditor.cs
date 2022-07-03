﻿using UnityEditor;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryDisplay), true)]
    /// <summary>
    /// Custom editor for the InventoryDisplay component
    /// </summary>
    public class InventoryDisplayEditor : Editor
    {
	    /// <summary>
	    ///     Gets the target inventory component.
	    /// </summary>
	    /// <value>The inventory target.</value>
	    public InventoryDisplay InventoryDisplayTarget => (InventoryDisplay)target;

	    /// <summary>
	    ///     Custom editor for the inventory panel.
	    /// </summary>
	    public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            // if there's a change in the inspector, we resize our inventory and grid, and redraw the whole thing.

            DrawPropertiesExcluding(serializedObject);

            // if for some reason we don't have a target inventory, we do nothing and exit
            if (InventoryDisplayTarget == null) return;

            // we add a button to manually empty the inventory
            EditorGUILayout.Space();
            if (GUILayout.Button("Auto setup inventory display panel"))
            {
                InventoryDisplayTarget.SetupInventoryDisplay();
                SceneView.RepaintAll();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SceneView.RepaintAll();
            }

            // we apply our changes
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
    }
}