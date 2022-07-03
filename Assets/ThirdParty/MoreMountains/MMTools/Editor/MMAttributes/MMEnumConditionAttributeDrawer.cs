using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Tools
{
    // original implementation by http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    [CustomPropertyDrawer(typeof(MMEnumConditionAttribute))]
    public class MMEnumConditionAttributeDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, string> cachedPaths = new();
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumConditionAttribute = (MMEnumConditionAttribute)attribute;
            var enabled = GetConditionAttributeResult(enumConditionAttribute, property);
            var previouslyEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!enumConditionAttribute.Hidden || enabled) EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = previouslyEnabled;
        }
#endif

        private bool GetConditionAttributeResult(MMEnumConditionAttribute enumConditionAttribute,
            SerializedProperty property)
        {
            var enabled = true;

            SerializedProperty enumProp;
            var enumPropPath = string.Empty;
            var propertyPath = property.propertyPath;

            if (!cachedPaths.TryGetValue(propertyPath, out enumPropPath))
            {
                enumPropPath = propertyPath.Replace(property.name, enumConditionAttribute.ConditionEnum);
                cachedPaths.Add(propertyPath, enumPropPath);
            }

            enumProp = property.serializedObject.FindProperty(enumPropPath);

            if (enumProp != null)
            {
                var currentEnum = enumProp.enumValueIndex;
                enabled = enumConditionAttribute.ContainsBitFlag(currentEnum);
            }
            else
            {
                Debug.LogWarning("No matching boolean found for ConditionAttribute in object: " +
                                 enumConditionAttribute.ConditionEnum);
            }

            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumConditionAttribute = (MMEnumConditionAttribute)attribute;
            var enabled = GetConditionAttributeResult(enumConditionAttribute, property);

            if (!enumConditionAttribute.Hidden || enabled)
                return EditorGUI.GetPropertyHeight(property, label);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}