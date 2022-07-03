using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    public class MMFInspectorGroupData
    {
        public MMFInspectorGroupAttribute GroupAttribute;
        public Color GroupColor;
        public HashSet<string> GroupHashSet = new();
        public bool GroupIsOpen;
        public List<SerializedProperty> PropertiesList = new();

        public void ClearGroup()
        {
            GroupAttribute = null;
            GroupHashSet.Clear();
            PropertiesList.Clear();
        }
    }

    public class MMF_FeedbackInspector
    {
        private const string _channelFieldName = "Channel";
        protected SerializedProperty _currentProperty;
        protected bool _expandGroupInspectors;
        protected MMF_Feedback _feedback;
        protected GUIContent _groupTitle = new();
        private bool _hasMMHiddenProperties;

        protected Rect _leftBorderRect;

        private string[] _mmHiddenPropertiesToHide;
        private bool _requiresConstantRepaint;
        protected Rect _setupRect;
        protected bool _shouldDrawBase = true;
        protected Rect _verticalGroup;
        protected Rect _widthRect;
        public bool DrawerInitialized;
        public Dictionary<string, MMFInspectorGroupData> GroupData = new();
        public List<SerializedProperty> PropertiesList = new();

        public virtual void OnEnable()
        {
            DrawerInitialized = false;
            PropertiesList.Clear();
            GroupData.Clear();

            var hiddenProperties = (MMFHiddenPropertiesAttribute[])_currentProperty.GetType()
                .GetCustomAttributes(typeof(MMFHiddenPropertiesAttribute), false);
            if (hiddenProperties != null && hiddenProperties.Length > 0 && hiddenProperties[0].PropertiesNames != null)
            {
                _mmHiddenPropertiesToHide = hiddenProperties[0].PropertiesNames;
                _hasMMHiddenProperties = true;
            }
        }

        public virtual void OnDisable()
        {
            foreach (var groupData in GroupData)
                if (groupData.Value != null)
                {
                    EditorPrefs.SetBool(
                        string.Format(
                            $"{groupData.Value.GroupAttribute.GroupName}{groupData.Value.PropertiesList[0].name}{_feedback.UniqueID}"),
                        groupData.Value.GroupIsOpen);
                    groupData.Value.ClearGroup();
                }
        }

        public virtual void Initialization(SerializedProperty currentProperty, MMF_Feedback feedback,
            bool expandGroupInspectors)
        {
            if (DrawerInitialized) return;

            _expandGroupInspectors = expandGroupInspectors;
            _currentProperty = currentProperty;
            _feedback = feedback;

            List<FieldInfo> fieldInfoList;
            MMFInspectorGroupAttribute previousGroupAttribute = default;
            var fieldInfoLength = MMF_FieldInfo.GetFieldInfo(feedback, out fieldInfoList);

            for (var i = 0; i < fieldInfoLength; i++)
            {
                var group =
                    Attribute.GetCustomAttribute(fieldInfoList[i], typeof(MMFInspectorGroupAttribute)) as
                        MMFInspectorGroupAttribute;

                MMFInspectorGroupData groupData;
                if (group == null)
                {
                    if (previousGroupAttribute != null && previousGroupAttribute.GroupAllFieldsUntilNextGroupAttribute)
                    {
                        _shouldDrawBase = false;
                        if (!GroupData.TryGetValue(previousGroupAttribute.GroupName, out groupData))
                        {
                            GroupData.Add(previousGroupAttribute.GroupName, new MMFInspectorGroupData
                            {
                                GroupAttribute = previousGroupAttribute,
                                GroupHashSet = new HashSet<string> { fieldInfoList[i].Name },
                                GroupColor = MMFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex)
                            });
                        }
                        else
                        {
                            groupData.GroupColor = MMFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex);
                            groupData.GroupHashSet.Add(fieldInfoList[i].Name);
                        }
                    }

                    continue;
                }

                previousGroupAttribute = group;

                if (!GroupData.TryGetValue(group.GroupName, out groupData))
                {
                    var fallbackOpenState = _expandGroupInspectors;
                    if (group.ClosedByDefault) fallbackOpenState = false;
                    var groupIsOpen =
                        EditorPrefs.GetBool(
                            string.Format($"{group.GroupName}{fieldInfoList[i].Name}{feedback.UniqueID}"),
                            fallbackOpenState);
                    GroupData.Add(group.GroupName, new MMFInspectorGroupData
                    {
                        GroupAttribute = group,
                        GroupColor = MMFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex),
                        GroupHashSet = new HashSet<string> { fieldInfoList[i].Name }, GroupIsOpen = groupIsOpen
                    });
                }
                else
                {
                    groupData.GroupHashSet.Add(fieldInfoList[i].Name);
                    groupData.GroupColor = MMFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex);
                }
            }


            if (currentProperty.NextVisible(true))
                do
                {
                    FillPropertiesList(currentProperty);
                } while (currentProperty.NextVisible(false));

            DrawerInitialized = true;
        }

        public void FillPropertiesList(SerializedProperty serializedProperty)
        {
            var shouldClose = false;

            foreach (var pair in GroupData)
                if (pair.Value.GroupHashSet.Contains(serializedProperty.name))
                {
                    var property = serializedProperty.Copy();
                    shouldClose = true;
                    pair.Value.PropertiesList.Add(property);
                    break;
                }

            if (!shouldClose)
            {
                var property = serializedProperty.Copy();
                PropertiesList.Add(property);
            }
        }

        public void DrawInspector(SerializedProperty currentProperty, MMF_Feedback feedback)
        {
            Initialization(currentProperty, feedback, _expandGroupInspectors);
            if (!DrawBase(currentProperty, feedback))
            {
                DrawContainer(feedback);
                DrawContents(feedback);
            }
        }

        protected virtual bool DrawBase(SerializedProperty currentProperty, MMF_Feedback feedback)
        {
            if (_shouldDrawBase || !feedback.DrawGroupInspectors)
            {
                DrawNoGroupInspector(currentProperty, feedback);
                return true;
            }

            return false;
        }

        protected virtual void DrawContainer(MMF_Feedback feedback)
        {
            if (PropertiesList.Count == 0) return;

            foreach (var pair in GroupData)
            {
                DrawVerticalLayout(() => DrawGroup(pair.Value, feedback), MMF_FeedbackInspectorStyle.ContainerStyle);
                EditorGUI.indentLevel = 0;
            }
        }

        protected virtual void DrawContents(MMF_Feedback feedback)
        {
            if (PropertiesList.Count == 0) return;

            EditorGUILayout.Space();
            for (var i = 1; i < PropertiesList.Count; i++)
                if (_hasMMHiddenProperties && !_mmHiddenPropertiesToHide.Contains(PropertiesList[i].name))
                    if (!DrawCustomInspectors(PropertiesList[i], feedback))
                        EditorGUILayout.PropertyField(PropertiesList[i], true);
        }

        protected virtual void DrawGroup(MMFInspectorGroupData groupData, MMF_Feedback feedback)
        {
            _verticalGroup = EditorGUILayout.BeginVertical();

            // we draw a colored line on the left
            _leftBorderRect.x = _verticalGroup.xMin + 5;
            _leftBorderRect.y = _verticalGroup.yMin - 0;
            _leftBorderRect.width = 3f;
            _leftBorderRect.height = _verticalGroup.height + 0;
            _leftBorderRect.xMin = 15f;
            _leftBorderRect.xMax = 18f;
            EditorGUI.DrawRect(_leftBorderRect, groupData.GroupColor);

            if (groupData.GroupAttribute.RequiresSetup && feedback.RequiresSetup)
            {
                // we draw a warning sign if needed
                _widthRect = EditorGUILayout.GetControlRect(false, 0);
                var setupRectWidth = 20f;
                _setupRect.x = _widthRect.xMax - setupRectWidth;
                _setupRect.y = _verticalGroup.yMin;
                _setupRect.width = setupRectWidth;
                _setupRect.height = 17f;

                EditorGUI.LabelField(_setupRect, MMF_PlayerStyling._setupRequiredIcon);
            }

            groupData.GroupIsOpen = EditorGUILayout.Foldout(groupData.GroupIsOpen, groupData.GroupAttribute.GroupName,
                true, MMF_FeedbackInspectorStyle.GroupStyle);

            if (groupData.GroupIsOpen)
            {
                EditorGUI.indentLevel = 0;

                for (var i = 0; i < groupData.PropertiesList.Count; i++)
                    DrawVerticalLayout(() => DrawChild(i), MMF_FeedbackInspectorStyle.BoxChildStyle);
            }

            EditorGUILayout.EndVertical();

            void DrawChild(int i)
            {
                if (_hasMMHiddenProperties &&
                    _mmHiddenPropertiesToHide.Contains(groupData.PropertiesList[i].name)) return;

                if (!feedback.HasChannel && groupData.PropertiesList[i].name == _channelFieldName) return;

                _groupTitle.text = ObjectNames.NicifyVariableName(groupData.PropertiesList[i].name);
                _groupTitle.tooltip = groupData.PropertiesList[i].tooltip;

                if (!DrawCustomInspectors(groupData.PropertiesList[i], feedback))
                    EditorGUILayout.PropertyField(groupData.PropertiesList[i], _groupTitle, true);
            }
        }

        public static void DrawVerticalLayout(Action action, GUIStyle style)
        {
            EditorGUILayout.BeginVertical(style);
            action();
            EditorGUILayout.EndVertical();
        }

        public void DrawNoGroupInspector(SerializedProperty currentProperty, MMF_Feedback feedback)
        {
            var endProp = currentProperty.GetEndProperty();

            while (currentProperty.NextVisible(true) && !EqualContents(endProp, currentProperty))
                if (currentProperty.depth <= 2)
                    if (!DrawCustomInspectors(currentProperty, feedback))
                        EditorGUILayout.PropertyField(currentProperty, true);
        }

        private bool DrawCustomInspectors(SerializedProperty currentProperty, MMF_Feedback feedback)
        {
            if (feedback.HasCustomInspectors)
                switch (currentProperty.type)
                {
                    case "MMF_Button":
                        var myButton = (MMF_Button)currentProperty.MMFGetObjectValue();
                        if (GUILayout.Button(myButton.ButtonText)) myButton.TargetMethod();
                        return true;
                }

            return false;
        }

        private bool EqualContents(SerializedProperty a, SerializedProperty b)
        {
            return SerializedProperty.EqualContents(a, b);
        }
    }
}