using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    [CustomEditor(typeof(CharacterAbility), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Adds custom labels to the Character inspector
    /// </summary>
    public class CharacterAbilityInspector : Editor
    {
        protected bool _foldout;
        protected bool _hasHiddenProperties;

        protected List<string> _propertiesToHide;

        private SerializedProperty AbilityStartSfx,
            AbilityInProgressSfx,
            AbilityStopSfx,
            AbilityStartFeedbacks,
            AbilityStopFeedbacks;

        protected virtual void OnEnable()
        {
            _propertiesToHide = new List<string>();

            AbilityStartSfx = serializedObject.FindProperty("AbilityStartSfx");
            AbilityInProgressSfx = serializedObject.FindProperty("AbilityInProgressSfx");
            AbilityStopSfx = serializedObject.FindProperty("AbilityStopSfx");
            AbilityStartFeedbacks = serializedObject.FindProperty("AbilityStartFeedbacks");
            AbilityStopFeedbacks = serializedObject.FindProperty("AbilityStopFeedbacks");

            var attributes =
                (MMHiddenPropertiesAttribute[])target.GetType()
                    .GetCustomAttributes(typeof(MMHiddenPropertiesAttribute), false);
            if (attributes != null)
                if (attributes.Length != 0)
                    if (attributes[0].PropertiesNames != null)
                    {
                        _propertiesToHide = new List<string>(attributes[0].PropertiesNames);
                        _hasHiddenProperties = true;
                    }
        }

        /// <summary>
        ///     When inspecting a Character, adds to the regular inspector some labels, useful for debugging
        /// </summary>
        public override void OnInspectorGUI()
        {
            var t = target as CharacterAbility;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            if (t.HelpBoxText() != "") EditorGUILayout.HelpBox(t.HelpBoxText(), MessageType.Info);

            DrawPropertiesExcluding(serializedObject, "AbilityStartSfx", "AbilityInProgressSfx", "AbilityStopSfx",
                "AbilityStartFeedbacks", "AbilityStopFeedbacks");

            EditorGUILayout.Space();

            EditorGUILayout.GetControlRect(true, 16f, EditorStyles.foldout);
            var foldRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && foldRect.Contains(Event.current.mousePosition))
            {
                _foldout = !_foldout;
                GUI.changed = true;
                Event.current.Use();
            }

            _foldout = EditorGUI.Foldout(foldRect, _foldout, "Ability Sounds");

            if (_foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(AbilityStartSfx);
                EditorGUILayout.PropertyField(AbilityInProgressSfx);
                EditorGUILayout.PropertyField(AbilityStopSfx);
                EditorGUI.indentLevel--;
            }

            if (_propertiesToHide.Count > 0)
            {
                if (_propertiesToHide.Count < 2) EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);
                if (!_propertiesToHide.Contains("AbilityStartFeedbacks"))
                    EditorGUILayout.PropertyField(AbilityStartFeedbacks);
                if (!_propertiesToHide.Contains("AbilityStopFeedbacks"))
                    EditorGUILayout.PropertyField(AbilityStopFeedbacks);
            }
            else
            {
                EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AbilityStartFeedbacks);
                EditorGUILayout.PropertyField(AbilityStopFeedbacks);
            }

            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }
    }
}