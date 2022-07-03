﻿using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class used to add a menu item and a shortcut to group objects together under a parent game object
    /// </summary>
    public class MMGroupSelection
    {
        /// <summary>
        ///     Creates a parent object and puts all selected transforms under it
        /// </summary>
        [MenuItem("Tools/More Mountains/Group Selection %g")]
        public static void GroupSelection()
        {
            if (!Selection.activeTransform) return;

            var groupObject = new GameObject();
            groupObject.name = "Group";

            Undo.RegisterCreatedObjectUndo(groupObject, "Group Selection");

            groupObject.transform.SetParent(Selection.activeTransform.parent, false);

            foreach (var selectedTransform in Selection.transforms)
                Undo.SetTransformParent(selectedTransform, groupObject.transform, "Group Selection");
            Selection.activeGameObject = groupObject;
        }
    }
}