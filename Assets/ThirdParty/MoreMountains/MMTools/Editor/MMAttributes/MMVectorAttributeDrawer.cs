using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Tools
{
    [CustomPropertyDrawer(typeof(MMVectorAttribute))]
    public class MMVectorLabelsAttributeDrawer : PropertyDrawer
    {
        protected const int padding = 375;
        protected static readonly GUIContent[] originalLabels = { new("X"), new("Y"), new("Z"), new("W") };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent guiContent)
        {
            var ratio = padding > Screen.width ? 2 : 1;
            return ratio * base.GetPropertyHeight(property, guiContent);
        }

#if UNITY_EDITOR
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent guiContent)
        {
            var vector = (MMVectorAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                float[] fieldArray = { property.vector2Value.x, property.vector2Value.y };
                fieldArray = DrawFields(rect, fieldArray, ObjectNames.NicifyVariableName(property.name),
                    EditorGUI.FloatField, vector);
                property.vector2Value = new Vector2(fieldArray[0], fieldArray[1]);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3)
            {
                float[] fieldArray = { property.vector3Value.x, property.vector3Value.y, property.vector3Value.z };
                fieldArray = DrawFields(rect, fieldArray, ObjectNames.NicifyVariableName(property.name),
                    EditorGUI.FloatField, vector);
                property.vector3Value = new Vector3(fieldArray[0], fieldArray[1], fieldArray[2]);
            }
            else if (property.propertyType == SerializedPropertyType.Vector4)
            {
                float[] fieldArray =
                {
                    property.vector4Value.x, property.vector4Value.y, property.vector4Value.z, property.vector4Value.w
                };
                fieldArray = DrawFields(rect, fieldArray, ObjectNames.NicifyVariableName(property.name),
                    EditorGUI.FloatField, vector);
                property.vector4Value = new Vector4(fieldArray[0], fieldArray[1], fieldArray[2]);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                int[] fieldArray = { property.vector2IntValue.x, property.vector2IntValue.y };
                fieldArray = DrawFields(rect, fieldArray, ObjectNames.NicifyVariableName(property.name),
                    EditorGUI.IntField, vector);
                property.vector2IntValue = new Vector2Int(fieldArray[0], fieldArray[1]);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                int[] array = { property.vector3IntValue.x, property.vector3IntValue.y, property.vector3IntValue.z };
                array = DrawFields(rect, array, ObjectNames.NicifyVariableName(property.name), EditorGUI.IntField,
                    vector);
                property.vector3IntValue = new Vector3Int(array[0], array[1], array[2]);
            }
        }
#endif

        protected T[] DrawFields<T>(Rect rect, T[] vector, string mainLabel, Func<Rect, GUIContent, T, T> fieldDrawer,
            MMVectorAttribute vectors)
        {
            var result = vector;

            var shortSpace = Screen.width < padding;

            var mainLabelRect = rect;
            mainLabelRect.width = EditorGUIUtility.labelWidth;
            if (shortSpace) mainLabelRect.height *= 0.5f;

            var fieldRect = rect;
            if (shortSpace)
            {
                fieldRect.height *= 0.5f;
                fieldRect.y += fieldRect.height;
                fieldRect.width = rect.width / vector.Length;
            }
            else
            {
                fieldRect.x += mainLabelRect.width;
                fieldRect.width = (rect.width - mainLabelRect.width) / vector.Length;
            }

            EditorGUI.LabelField(mainLabelRect, mainLabel);

            for (var i = 0; i < vector.Length; i++)
            {
                var label = vectors.Labels.Length > i ? new GUIContent(vectors.Labels[i]) : originalLabels[i];
                var labelSize = EditorStyles.label.CalcSize(label);
                EditorGUIUtility.labelWidth = Mathf.Max(labelSize.x + 5, 0.3f * fieldRect.width);
                result[i] = fieldDrawer(fieldRect, label, vector[i]);
                fieldRect.x += fieldRect.width;
            }

            EditorGUIUtility.labelWidth = 0;
            return result;
        }
    }
}