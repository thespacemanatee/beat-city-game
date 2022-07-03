using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Custom editor for the FloatController, conditional hiding and dropdown fill
    /// </summary>
    [CustomEditor(typeof(FloatController), true)]
    [CanEditMultipleObjects]
    public class FloatControllerEditor : Editor
    {
        protected const int _lineHeight = 20;
        protected const int _lineMargin = 2;
        protected const int _numberOfLines = 1;

        protected SerializedProperty _Amplitude;

        protected SerializedProperty _AudioAnalyzer;
        protected SerializedProperty _AudioAnalyzerMode;
        protected SerializedProperty _AudioAnalyzerMultiplier;

        protected Rect _barRect;
        protected SerializedProperty _BeatID;

        protected SerializedProperty _ChoiceIndex;
        protected SerializedProperty _CurrentValue;
        protected SerializedProperty _CurrentValueNormalized;

        protected SerializedProperty _Curve;
        protected SerializedProperty _DisableAfterOneTime;
        protected SerializedProperty _DisableAfterToDestination;
        protected SerializedProperty _DisableGameObjectAfterOneTime;

        protected SerializedProperty _DrivenLevel;
        protected SerializedProperty _Duration;
        protected SerializedProperty _Frequency;

        protected SerializedProperty _InitialValue;
        protected SerializedProperty _MaxValue;
        protected SerializedProperty _MinValue;
        protected Color _mmRed = MMColors.Orangered;
        protected Color _mmYellow = new(1f, 0.7686275f, 0f);
        protected SerializedProperty _NormalizedLevelID;
        protected SerializedProperty _OneTimeAmplitude;
        protected SerializedProperty _OneTimeButton;
        protected SerializedProperty _OneTimeCurve;

        protected SerializedProperty _OneTimeDuration;
        protected SerializedProperty _OneTimeRemapMax;
        protected SerializedProperty _OneTimeRemapMin;
        protected SerializedProperty _PingPongPauseDuration;
        protected Color _progressBarBackground = new(0, 0, 0, 0.5f);
        protected SerializedProperty _PropertyName;
        protected SerializedProperty _RemapNoiseOne;
        protected SerializedProperty _RemapNoiseValues;
        protected SerializedProperty _RemapNoiseZero;
        protected SerializedProperty _Shift;
        protected SerializedProperty _TargetObject;
        protected SerializedProperty _ToDestinationButton;
        protected SerializedProperty _ToDestinationCurve;
        protected SerializedProperty _ToDestinationDuration;

        protected SerializedProperty _ToDestinationValue;

        /// <summary>
        ///     On enable, grabs our serialized properties
        /// </summary>
        protected virtual void OnEnable()
        {
            var myTarget = (FloatController)target;

            _TargetObject = serializedObject.FindProperty("TargetObject");

            _Curve = serializedObject.FindProperty("Curve");
            _MinValue = serializedObject.FindProperty("MinValue");
            _MaxValue = serializedObject.FindProperty("MaxValue");
            _Duration = serializedObject.FindProperty("Duration");
            _PingPongPauseDuration = serializedObject.FindProperty("PingPongPauseDuration");

            _Amplitude = serializedObject.FindProperty("Amplitude");
            _Frequency = serializedObject.FindProperty("Frequency");
            _Shift = serializedObject.FindProperty("Shift");
            _RemapNoiseValues = serializedObject.FindProperty("RemapNoiseValues");
            _RemapNoiseZero = serializedObject.FindProperty("RemapNoiseZero");
            _RemapNoiseOne = serializedObject.FindProperty("RemapNoiseOne");

            _OneTimeDuration = serializedObject.FindProperty("OneTimeDuration");
            _OneTimeAmplitude = serializedObject.FindProperty("OneTimeAmplitude");
            _OneTimeRemapMin = serializedObject.FindProperty("OneTimeRemapMin");
            _OneTimeRemapMax = serializedObject.FindProperty("OneTimeRemapMax");
            _OneTimeCurve = serializedObject.FindProperty("OneTimeCurve");
            _DisableAfterOneTime = serializedObject.FindProperty("DisableAfterOneTime");
            _DisableGameObjectAfterOneTime = serializedObject.FindProperty("DisableGameObjectAfterOneTime");
            _OneTimeButton = serializedObject.FindProperty("OneTimeButton");

            _DrivenLevel = serializedObject.FindProperty("DrivenLevel");

            _ToDestinationValue = serializedObject.FindProperty("ToDestinationValue");
            _ToDestinationDuration = serializedObject.FindProperty("ToDestinationDuration");
            _ToDestinationCurve = serializedObject.FindProperty("ToDestinationCurve");
            _DisableAfterToDestination = serializedObject.FindProperty("DisableAfterToDestination");
            _ToDestinationButton = serializedObject.FindProperty("ToDestinationButton");

            _InitialValue = serializedObject.FindProperty("InitialValue");
            _CurrentValue = serializedObject.FindProperty("CurrentValue");
            _CurrentValueNormalized = serializedObject.FindProperty("CurrentValueNormalized");

            _ChoiceIndex = serializedObject.FindProperty("ChoiceIndex");
            _PropertyName = serializedObject.FindProperty("PropertyName");

            _AudioAnalyzer = serializedObject.FindProperty("AudioAnalyzer");
            _AudioAnalyzerMode = serializedObject.FindProperty("AudioAnalyzerMode");
            _NormalizedLevelID = serializedObject.FindProperty("NormalizedLevelID");
            _BeatID = serializedObject.FindProperty("BeatID");
            _AudioAnalyzerMultiplier = serializedObject.FindProperty("AudioAnalyzerMultiplier");

            VerifyChosenIndex();
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        protected virtual void OnDisable()
        {
            //BindPropertyName();
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        protected virtual void BindPropertyName()
        {
            var myTarget = (FloatController)target;

            if (myTarget.ChoiceIndex > myTarget.AttributeNames.Length - 1)
            {
                _ChoiceIndex.intValue = 0;
                _PropertyName.stringValue = FloatController._undefinedString;
            }
            else
            {
                _PropertyName.stringValue = myTarget.AttributeNames[myTarget.ChoiceIndex];
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual void VerifyChosenIndex()
        {
            var myTarget = (FloatController)target;

            // determine choice index
            var index = 0;
            var found = false;
            foreach (var attName in myTarget.AttributeNames)
            {
                if (attName == myTarget.PropertyName)
                {
                    _ChoiceIndex.intValue = index;
                    found = true;
                }

                index++;
            }

            if (!found)
            {
                _ChoiceIndex.intValue = 0;
                _PropertyName.stringValue = FloatController._undefinedString;
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnAfterAssemblyReload()
        {
            var myTarget = (FloatController)target;
            myTarget.FillDropDownList();
            VerifyChosenIndex();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Draws a custom conditional inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "Modified FloatController");

            var myTarget = (FloatController)target;

            EditorGUILayout.PropertyField(_TargetObject);
            if (myTarget.AttributeNames != null)
                if (myTarget.AttributeNames.Length > 0)
                {
                    // draws a dropdown with all our properties
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Property");

                    _ChoiceIndex.intValue = EditorGUILayout.Popup(myTarget.ChoiceIndex, myTarget.AttributeNames);
                    BindPropertyName();
                    EditorGUILayout.EndHorizontal();

                    DrawPropertiesExcluding(serializedObject, "m_Script", "TargetObject", "Curve", "MinValue",
                        "MaxValue", "Duration", "Amplitude", "RemapNoiseValues", "RemapNoiseZero", "RemapNoiseOne",
                        "Frequency", "Shift", "InitialValue", "CurrentValue", "CurrentValueNormalized",
                        "PingPongPauseDuration", "OneTimeDuration", "OneTimeAmplitude", "OneTimeRemapMin",
                        "OneTimeRemapMax", "OneTimeCurve", "OneTimeButton", "DisableAfterOneTime",
                        "DisableGameObjectAfterOneTime", "AudioAnalyzer", "AudioAnalyzerMode", "BeatID",
                        "NormalizedLevelID", "AudioAnalyzerMultiplier", "DisableAfterToDestination", "DrivenLevel",
                        "ToDestinationDuration", "ToDestinationValue", "ToDestinationCurve", "ToDestinationButton");

                    if (myTarget.ControlMode == FloatController.ControlModes.PingPong)
                    {
                        EditorGUILayout.PropertyField(_Curve);
                        EditorGUILayout.PropertyField(_MinValue);
                        EditorGUILayout.PropertyField(_MaxValue);
                        EditorGUILayout.PropertyField(_Duration);
                        EditorGUILayout.PropertyField(_PingPongPauseDuration);
                    }
                    else if (myTarget.ControlMode == FloatController.ControlModes.Random)
                    {
                        EditorGUILayout.PropertyField(_Amplitude);
                        EditorGUILayout.PropertyField(_Frequency);
                        EditorGUILayout.PropertyField(_Shift);
                        EditorGUILayout.PropertyField(_RemapNoiseValues);
                        EditorGUILayout.PropertyField(_RemapNoiseZero);
                        EditorGUILayout.PropertyField(_RemapNoiseOne);
                    }
                    else if (myTarget.ControlMode == FloatController.ControlModes.Driven)
                    {
                        EditorGUILayout.PropertyField(_DrivenLevel);
                    }
                    else if (myTarget.ControlMode == FloatController.ControlModes.OneTime)
                    {
                        EditorGUILayout.PropertyField(_OneTimeDuration);
                        EditorGUILayout.PropertyField(_OneTimeAmplitude);
                        EditorGUILayout.PropertyField(_OneTimeRemapMin);
                        EditorGUILayout.PropertyField(_OneTimeRemapMax);
                        EditorGUILayout.PropertyField(_OneTimeCurve);
                        EditorGUILayout.PropertyField(_DisableAfterOneTime);
                        EditorGUILayout.PropertyField(_DisableGameObjectAfterOneTime);
                        EditorGUILayout.PropertyField(_OneTimeButton);
                    }
                    else if (myTarget.ControlMode == FloatController.ControlModes.AudioAnalyzer)
                    {
                        EditorGUILayout.PropertyField(_AudioAnalyzer);
                        EditorGUILayout.PropertyField(_AudioAnalyzerMode);
                        if (myTarget.AudioAnalyzerMode == FloatController.AudioAnalyzerModes.Beat)
                            EditorGUILayout.PropertyField(_BeatID);
                        else
                            EditorGUILayout.PropertyField(_NormalizedLevelID);
                        EditorGUILayout.PropertyField(_AudioAnalyzerMultiplier);
                    }
                    else if (myTarget.ControlMode == FloatController.ControlModes.ToDestination)
                    {
                        EditorGUILayout.PropertyField(_ToDestinationDuration);
                        EditorGUILayout.PropertyField(_ToDestinationValue);
                        EditorGUILayout.PropertyField(_ToDestinationCurve);
                        EditorGUILayout.PropertyField(_DisableAfterToDestination);
                        EditorGUILayout.PropertyField(_ToDestinationButton);
                    }

                    EditorGUILayout.PropertyField(_InitialValue);
                    EditorGUILayout.PropertyField(_CurrentValue);
                    EditorGUILayout.PropertyField(_CurrentValueNormalized);
                }


            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                _barRect = EditorGUILayout.GetControlRect();
                DrawLevelProgressBar(_barRect, myTarget.CurrentValueNormalized, _mmYellow, _mmRed);
            }
        }

        protected virtual void DrawLevelProgressBar(Rect position, float level, Color frontColor, Color negativeColor)
        {
            var levelLabelRect = new Rect(position.x, position.y + (_lineHeight + _lineMargin) * (_numberOfLines - 1),
                position.width, _lineHeight);
            var levelValueRect = new Rect(position.x - 15 + EditorGUIUtility.labelWidth + 4,
                position.y + (_lineHeight + _lineMargin) * (_numberOfLines - 1), position.width, _lineHeight);

            var progressX = position.x - 5 + EditorGUIUtility.labelWidth + 60;
            var progressY = position.y + (_lineHeight + _lineMargin) * (_numberOfLines - 1) + 6;
            var progressHeight = 10f;
            var fullProgressWidth = position.width - EditorGUIUtility.labelWidth - 60 + 5;

            var negative = false;
            var displayLevel = level;
            if (level < 0f)
            {
                negative = true;
                level = -level;
            }

            var progressLevel = Mathf.Clamp01(level);
            var levelProgressBg = new Rect(progressX, progressY, fullProgressWidth, progressHeight);
            var progressWidth = MMMaths.Remap(progressLevel, 0f, 1f, 0f, fullProgressWidth);
            var levelProgressFront = new Rect(progressX, progressY, progressWidth, progressHeight);

            EditorGUI.LabelField(levelLabelRect, new GUIContent("Level"));
            EditorGUI.LabelField(levelValueRect, new GUIContent(displayLevel.ToString("F4")));
            EditorGUI.DrawRect(levelProgressBg, _progressBarBackground);
            if (negative)
                EditorGUI.DrawRect(levelProgressFront, negativeColor);
            else
                EditorGUI.DrawRect(levelProgressFront, frontColor);
        }
    }
}