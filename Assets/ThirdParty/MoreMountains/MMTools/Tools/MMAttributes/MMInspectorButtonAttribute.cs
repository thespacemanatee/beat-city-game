using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Tools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MMInspectorButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public MMInspectorButtonAttribute(string MethodName)
        {
            this.MethodName = MethodName;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MMInspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var inspectorButtonAttribute = (MMInspectorButtonAttribute)attribute;

            var buttonLength = position.width;
            var buttonRect = new Rect(position.x, position.y, buttonLength, position.height);
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            if (GUI.Button(buttonRect, inspectorButtonAttribute.MethodName))
            {
                var eventOwnerType = prop.serializedObject.targetObject.GetType();
                var eventName = inspectorButtonAttribute.MethodName;

                if (_eventMethodInfo == null)
                    _eventMethodInfo = eventOwnerType.GetMethod(eventName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (_eventMethodInfo != null)
                    _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                else
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName,
                        eventOwnerType));
            }
        }
    }
#endif
}