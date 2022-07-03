using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Tools
{
    [CustomPropertyDrawer(typeof(MMColorAttribute))]
    public class MMColorAttributeDrawer : PropertyDrawer
    {
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var color = (attribute as MMColorAttribute).color;
            var prev = GUI.color;
            GUI.color = color;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.color = prev;
        }
#endif
    }
}