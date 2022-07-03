﻿// Original FindMissingScriptsRecursively script by SimTex and Clement
// http://wiki.unity3d.com/index.php?title=FindMissingScripts

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    public class MMFindMissingScriptsRecursively : EditorWindow
    {
        private static int go_count, components_count, missing_count;

#if UNITY_EDITOR
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects")) FindInSelected();
        }
#endif

        [MenuItem("Window/FindMissingScriptsRecursively")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MMFindMissingScriptsRecursively));
        }

        private static void FindInSelected()
        {
            var go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (var g in go) FindInGO(g);
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count,
                components_count, missing_count));
        }

        private static void FindInGO(GameObject g)
        {
            go_count++;
            var components = g.GetComponents<Component>();
            for (var i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    var s = g.name;
                    var t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }

                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }

            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform) FindInGO(childT.gameObject);
        }
    }
}
#endif