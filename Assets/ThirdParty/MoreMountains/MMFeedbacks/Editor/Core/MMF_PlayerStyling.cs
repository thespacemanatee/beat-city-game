using System;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     A class used to regroup most of the styling options for the MMFeedback editors
    /// </summary>
    public static class MMF_PlayerStyling
    {
        public static readonly GUIStyle SmallTickbox = new("ShurikenToggle");

        private static readonly Color _splitterdark = new(0.12f, 0.12f, 0.12f, 1.333f);
        private static readonly Color _splitterlight = new(0.6f, 0.6f, 0.6f, 1.333f);

        private static readonly Color _headerbackgrounddark = new(0.1f, 0.1f, 0.1f, 0.2f);
        private static readonly Color _headerbackgroundlight = new(1f, 1f, 1f, 0.4f);

        private static readonly Color _reorderdark = new(1f, 1f, 1f, 0.2f);
        private static readonly Color _reorderlight = new(0.1f, 0.1f, 0.1f, 0.2f);

        private static readonly Color _timingDark = new(1f, 1f, 1f, 0.5f);
        private static readonly Color _timingLight = new(0f, 0f, 0f, 0.5f);
        private static readonly Color _targetLabelColor = new(1f, 1f, 1f, 0.4f);

        private static readonly Texture2D _paneoptionsicondark;
        private static readonly Texture2D _paneoptionsiconlight;

        private static Rect _splitterRect;

        private static readonly GUIStyle _timingStyle = new();

        private static Rect _backgroundRect;
        private static Rect _progressRect;
        private static Rect _timingRect;
        private static Rect _reorderRect;
        private static Rect _labelRect;
        private static Rect _foldoutRect;
        private static Rect _toggleRect;
        private static Rect _directionRect;
        private static Rect _setupRect;
        private static Texture2D _menuIcon;
        private static Rect _menuRect;
        private static Rect _workRect;
        private static Rect _colorRect;
        private static Rect _genericMenuRect;
        private static Color _headerBackgroundColor;
        private static Color _barColor;
        private static GUIContent _directionUpIcon;
        private static GUIStyle _targetLabelStyle;
        private static GUIContent _directionDownIcon;
        public static GUIContent _setupRequiredIcon;
        private static GenericMenu _genericMenu;

        static MMF_PlayerStyling()
        {
            _paneoptionsicondark = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
            _paneoptionsiconlight = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
        }

        public static Color Splitter => EditorGUIUtility.isProSkin ? _splitterdark : _splitterlight;

        public static Color HeaderBackground =>
            EditorGUIUtility.isProSkin ? _headerbackgrounddark : _headerbackgroundlight;

        public static Color Reorder => EditorGUIUtility.isProSkin ? _reorderdark : _reorderlight;

        public static Texture2D PaneOptionsIcon =>
            EditorGUIUtility.isProSkin ? _paneoptionsicondark : _paneoptionsiconlight;

        public static void CacheStyling()
        {
            _menuIcon = PaneOptionsIcon;
            _menuRect = new Rect();
            _colorRect = new Rect();
            _directionRect = new Rect();
            _setupRect = new Rect();
            _timingRect = new Rect();
            _directionUpIcon = new GUIContent(Resources.Load("FeelArrowUp") as Texture);
            _directionDownIcon = new GUIContent(Resources.Load("FeelArrowDown") as Texture);
            _setupRequiredIcon = new GUIContent(Resources.Load("FeelSetupRequired") as Texture);
            _genericMenu = new GenericMenu();
            _targetLabelStyle = new GUIStyle(GUI.skin.label);
            _targetLabelStyle.alignment = TextAnchor.MiddleRight;
            _targetLabelStyle.normal.textColor = _targetLabelColor;
        }

        /// <summary>
        ///     Simply drow a splitter line and a title bellow
        /// </summary>
        public static void DrawSection(string title)
        {
            EditorGUILayout.Space();
            DrawSplitter();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        /// <summary>
        ///     Draw a separator line
        /// </summary>
        public static void DrawSplitter()
        {
            // Helper to draw a separator line

            _splitterRect = GUILayoutUtility.GetRect(1f, 1f);

            _splitterRect.xMin = 0f;
            _splitterRect.width += 4f;

            if (Event.current.type != EventType.Repaint) return;

            EditorGUI.DrawRect(_splitterRect, Splitter);
        }

        /// <summary>
        ///     Draw a header similar to the one used for the post-process stack
        /// </summary>
        public static Rect DrawSimpleHeader(ref bool expanded, ref bool activeField, string title)
        {
            var e = Event.current;

            // Initialize Rects

            _backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            _reorderRect = _backgroundRect;
            _reorderRect.xMin -= 8f;
            _reorderRect.y += 5f;
            _reorderRect.width = 9f;
            _reorderRect.height = 9f;

            _labelRect = _backgroundRect;
            _labelRect.xMin += 32f;
            _labelRect.xMax -= 20f;

            _foldoutRect = _backgroundRect;
            _foldoutRect.y += 1f;
            _foldoutRect.width = 13f;
            _foldoutRect.height = 13f;

            _toggleRect = _backgroundRect;
            _toggleRect.x += 16f;
            _toggleRect.y += 2f;
            _toggleRect.width = 13f;
            _toggleRect.height = 13f;

            // Background rect should be full-width
            _backgroundRect.xMin = 0f;
            _backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(_backgroundRect, HeaderBackground);

            // Foldout
            expanded = GUI.Toggle(_foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title
            EditorGUI.LabelField(_labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            activeField = GUI.Toggle(_toggleRect, activeField, GUIContent.none, SmallTickbox);

            // Handle events

            if (e.type == EventType.MouseDown && _labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return _backgroundRect;
        }

        /// <summary>
        ///     Draw a header similar to the one used for the post-process stack
        /// </summary>
        public static Rect DrawHeader(ref bool expanded, ref bool activeField, string title, Color feedbackColor,
            Action<GenericMenu> fillGenericMenu,
            float startedAt, float duration, float totalDuration, MMFeedbackTiming timing, bool pause,
            bool requiresSetup, string requiredTarget, Color displayColor, MMF_Player host)
        {
            var thisTime = timing.TimescaleMode == TimescaleModes.Scaled ? Time.time : Time.unscaledTime;
            var thisDeltaTime = timing.TimescaleMode == TimescaleModes.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;

            var e = Event.current;

            // Initialize Rects
            _backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            _progressRect = GUILayoutUtility.GetRect(1f, 2f);

            var offset = 4f;

            _reorderRect = _backgroundRect;
            _reorderRect.xMin -= 8f;
            _reorderRect.y += 5f;
            _reorderRect.width = 9f;
            _reorderRect.height = 9f;

            _labelRect = _backgroundRect;
            _labelRect.xMin += 32f + offset;
            _labelRect.xMax -= 20f;

            _foldoutRect = _backgroundRect;
            _foldoutRect.y += 1f;
            _foldoutRect.xMin += offset;
            _foldoutRect.width = 13f;
            _foldoutRect.height = 13f;

            _toggleRect = _backgroundRect;
            _toggleRect.x += 16f;
            _toggleRect.xMin += offset;
            _toggleRect.y += 2f;
            _toggleRect.width = 13f;
            _toggleRect.height = 13f;

            _timingStyle.normal.textColor = EditorGUIUtility.isProSkin ? _timingDark : _timingLight;
            _timingStyle.alignment = TextAnchor.MiddleRight;

            _colorRect.x = _labelRect.xMin;
            _colorRect.y = _labelRect.yMin;
            _colorRect.width = 5f;
            _colorRect.height = 17f;
            _colorRect.xMin = 0f;
            _colorRect.xMax = 5f;
            EditorGUI.DrawRect(_colorRect, feedbackColor);

            // Background rect should be full-width
            _backgroundRect.xMin = 0f;
            _backgroundRect.width += 4f;

            _progressRect.xMin = 0f;
            _progressRect.width += 4f;

            _headerBackgroundColor = Color.white;
            // Background - if color is white we draw the default color
            if (!pause)
                _headerBackgroundColor = HeaderBackground;
            else
                _headerBackgroundColor = feedbackColor;

            if (displayColor != Color.black) _headerBackgroundColor = displayColor;

            EditorGUI.DrawRect(_backgroundRect, _headerBackgroundColor);

            // Foldout
            expanded = GUI.Toggle(_foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title ----------------------------------------------------------------------------------------------------

            using (new EditorGUI.DisabledScope(!activeField))
            {
                EditorGUI.LabelField(_labelRect, title, EditorStyles.boldLabel);
            }

            // Direction ----------------------------------------------------------------------------------------------

            var directionRectWidth = 70f;
            _directionRect.x = _labelRect.xMax - directionRectWidth;
            _directionRect.y = _labelRect.yMin;
            _directionRect.width = directionRectWidth;
            _directionRect.height = 17f;
            _directionRect.xMin = _labelRect.xMax - directionRectWidth;
            _directionRect.xMax = _labelRect.xMax;

            if (timing.MMFeedbacksDirectionCondition ==
                MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenBackwards)
                EditorGUI.LabelField(_directionRect, _directionUpIcon);

            if (timing.MMFeedbacksDirectionCondition ==
                MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenForwards)
                EditorGUI.LabelField(_directionRect, _directionDownIcon);

            if (requiresSetup)
            {
                var setupRectWidth = 90f;
                _setupRect.x = _labelRect.xMax - setupRectWidth;
                _setupRect.y = _labelRect.yMin;
                _setupRect.width = setupRectWidth;
                _setupRect.height = 17f;
                _setupRect.xMin = _labelRect.xMax - setupRectWidth;
                _setupRect.xMax = _labelRect.xMax;

                EditorGUI.LabelField(_setupRect, _setupRequiredIcon);
            }
            else
            {
                // otherwise we draw the name of our target
                var setupRectWidth = _labelRect.width / 2f;
                _setupRect.x = _labelRect.xMax - setupRectWidth - 73f;
                _setupRect.y = _labelRect.yMin;
                _setupRect.width = setupRectWidth;
                _setupRect.height = 17f;

                EditorGUI.LabelField(_setupRect, requiredTarget, _targetLabelStyle);
            }

            // Time -----------------------------------------------------------------------------------------------------

            var timingInfo = "";
            var displayTotal = false;
            if (host.DisplayFullDurationDetails)
            {
                if (timing.InitialDelay != 0)
                {
                    timingInfo += host.ApplyTimeMultiplier(timing.InitialDelay) + "s + ";
                    displayTotal = true;
                }

                timingInfo += duration.ToString("F2") + "s";

                if (timing.NumberOfRepeats != 0)
                {
                    var delayBetweenRepeats = host.ApplyTimeMultiplier(timing.DelayBetweenRepeats);

                    timingInfo += " + " + timing.NumberOfRepeats + " x ";
                    if (timing.DelayBetweenRepeats > 0) timingInfo += "(";
                    timingInfo += duration + "s";
                    if (timing.DelayBetweenRepeats > 0)
                        timingInfo += " + " + host.ApplyTimeMultiplier(timing.DelayBetweenRepeats) + "s )";
                    displayTotal = true;
                }

                if (displayTotal) timingInfo += " = " + totalDuration.ToString("F2") + "s";
            }
            else
            {
                timingInfo = totalDuration.ToString("F2") + "s";
            }

            var timingRectWidth = 150f;

            _timingRect.x = _labelRect.xMax - timingRectWidth;
            _timingRect.y = _labelRect.yMin;
            _timingRect.width = timingRectWidth;
            _timingRect.height = 17f;

            _timingRect.xMin = _labelRect.xMax - timingRectWidth;
            _timingRect.xMax = _labelRect.xMax;
            EditorGUI.LabelField(_timingRect, timingInfo, _timingStyle);

            // Progress bar
            if (totalDuration == 0f) totalDuration = 0.1f;

            if (startedAt == 0f) startedAt = 0.001f;
            if (host.IsPlaying && startedAt > 0f && thisTime - startedAt < totalDuration + 0.05f)
            {
                var fullWidth = _progressRect.width;
                if (totalDuration == 0f) totalDuration = 0.1f;
                var percent = (thisTime - startedAt) / totalDuration * 100f;
                _progressRect.width = percent * fullWidth / 100f;
                _barColor = Color.white;
                if (thisTime - startedAt > totalDuration) _barColor = Color.yellow;
                EditorGUI.DrawRect(_progressRect, _barColor);
            }
            else
            {
                EditorGUI.DrawRect(_progressRect, _headerBackgroundColor);
            }

            // Active checkbox
            activeField = GUI.Toggle(_toggleRect, activeField, GUIContent.none, SmallTickbox);


            _menuRect.x = _labelRect.xMax + 4f;
            _menuRect.y = _labelRect.y + 4f;
            _menuRect.width = _menuIcon.width;
            _menuRect.height = _menuIcon.height;

            // Dropdown menu icon
            GUI.DrawTexture(_menuRect, _menuIcon);

            for (var i = 0; i < 3; i++)
            {
                _workRect = _reorderRect;
                _workRect.height = 1;
                _workRect.y = _reorderRect.y + _reorderRect.height * (i / 3.0f);
                EditorGUI.DrawRect(_workRect, Reorder);
            }

            // Handle events

            if (e.type == EventType.MouseDown)
                if (_menuRect.Contains(e.mousePosition))
                {
                    fillGenericMenu(_genericMenu);

                    _genericMenuRect.x = _menuRect.x;
                    _genericMenuRect.y = _menuRect.yMax;
                    _genericMenuRect.width = 0f;
                    _genericMenuRect.height = 0f;
                    _genericMenu.DropDown(_genericMenuRect);
                    e.Use();
                }

            if (e.type == EventType.MouseDown && _labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return _backgroundRect;
        }

        public static void CreateColorTexture(this Texture2D texture2D, Color32 color)
        {
            var colorArray = texture2D.GetPixels32();

            for (var i = 0; i < colorArray.Length; ++i) colorArray[i] = color;
            texture2D.SetPixels32(colorArray);
            texture2D.Apply();
        }
    }
}