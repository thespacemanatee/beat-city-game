using System;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     A class used to regroup most of the styling options for the MMFeedback editors
    /// </summary>
    public static class MMFeedbackStyling
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

        private static readonly Texture2D _paneoptionsicondark;
        private static readonly Texture2D _paneoptionsiconlight;

        private static readonly GUIStyle _timingStyle = new();

        static MMFeedbackStyling()
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

            var rect = GUILayoutUtility.GetRect(1f, 1f);

            rect.xMin = 0f;
            rect.width += 4f;

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, Splitter);
        }

        /// <summary>
        ///     Draw a header similar to the one used for the post-process stack
        /// </summary>
        public static Rect DrawSimpleHeader(ref bool expanded, ref bool activeField, string title)
        {
            var e = Event.current;

            // Initialize Rects

            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var reorderRect = backgroundRect;
            reorderRect.xMin -= 8f;
            reorderRect.y += 5f;
            reorderRect.width = 9f;
            reorderRect.height = 9f;

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            var menuIcon = PaneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, HeaderBackground);

            // Foldout
            expanded = GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            activeField = GUI.Toggle(toggleRect, activeField, GUIContent.none, SmallTickbox);

            // Handle events

            if (e.type == EventType.MouseDown && labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return backgroundRect;
        }

        /// <summary>
        ///     Draw a header similar to the one used for the post-process stack
        /// </summary>
        public static Rect DrawHeader(ref bool expanded, ref bool activeField, string title, Color feedbackColor,
            Action<GenericMenu> fillGenericMenu,
            float startedAt, float duration, float totalDuration, MMFeedbackTiming timing, bool pause, MMFeedbacks host)
        {
            var thisTime = timing.TimescaleMode == TimescaleModes.Scaled ? Time.time : Time.unscaledTime;
            var thisDeltaTime = timing.TimescaleMode == TimescaleModes.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;

            var e = Event.current;

            // Initialize Rects
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var progressRect = GUILayoutUtility.GetRect(1f, 2f);

            var offset = 4f;

            var reorderRect = backgroundRect;
            reorderRect.xMin -= 8f;
            reorderRect.y += 5f;
            reorderRect.width = 9f;
            reorderRect.height = 9f;

            var labelRect = backgroundRect;
            labelRect.xMin += 32f + offset;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.xMin += offset;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.xMin += offset;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            var menuIcon = PaneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            _timingStyle.normal.textColor = EditorGUIUtility.isProSkin ? _timingDark : _timingLight;
            _timingStyle.alignment = TextAnchor.MiddleRight;

            var colorRect = new Rect(labelRect.xMin, labelRect.yMin, 5f, 17f);
            colorRect.xMin = 0f;
            colorRect.xMax = 5f;
            EditorGUI.DrawRect(colorRect, feedbackColor);

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            progressRect.xMin = 0f;
            progressRect.width += 4f;

            var headerBackgroundColor = Color.white;
            // Background - if color is white we draw the default color
            if (!pause)
                headerBackgroundColor = HeaderBackground;
            else
                headerBackgroundColor = feedbackColor;
            EditorGUI.DrawRect(backgroundRect, headerBackgroundColor);

            // Foldout
            expanded = GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);

            // Title ----------------------------------------------------------------------------------------------------

            using (new EditorGUI.DisabledScope(!activeField))
            {
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            }

            // Direction ----------------------------------------------------------------------------------------------

            var directionRectWidth = 70f;
            var directionRect = new Rect(labelRect.xMax - directionRectWidth, labelRect.yMin, directionRectWidth, 17f);
            directionRect.xMin = labelRect.xMax - directionRectWidth;
            directionRect.xMax = labelRect.xMax;

            if (timing.MMFeedbacksDirectionCondition ==
                MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenBackwards)
            {
                var arrowUpIcon = Resources.Load("FeelArrowUp") as Texture;
                var directionIcon = new GUIContent(arrowUpIcon);
                EditorGUI.LabelField(directionRect, directionIcon);
            }

            if (timing.MMFeedbacksDirectionCondition ==
                MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenForwards)
            {
                var arrowDownIcon = Resources.Load("FeelArrowDown") as Texture;
                var directionIcon = new GUIContent(arrowDownIcon);
                EditorGUI.LabelField(directionRect, directionIcon);
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

            //"[ 2s + 3 x (4s + 1s) ]"

            var timingRectWidth = 150f;
            var timingRect = new Rect(labelRect.xMax - timingRectWidth, labelRect.yMin, timingRectWidth, 17f);
            timingRect.xMin = labelRect.xMax - timingRectWidth;
            timingRect.xMax = labelRect.xMax;
            EditorGUI.LabelField(timingRect, timingInfo, _timingStyle);

            // Progress bar
            if (totalDuration == 0f) totalDuration = 0.1f;

            if (startedAt == 0f) startedAt = 0.001f;
            if (startedAt > 0f && thisTime - startedAt < totalDuration + 0.05f)
            {
                var fullWidth = progressRect.width;
                if (totalDuration == 0f) totalDuration = 0.1f;
                var percent = (thisTime - startedAt) / totalDuration * 100f;
                progressRect.width = percent * fullWidth / 100f;
                var barColor = Color.white;
                if (thisTime - startedAt > totalDuration) barColor = Color.yellow;
                EditorGUI.DrawRect(progressRect, barColor);
            }
            else
            {
                EditorGUI.DrawRect(progressRect, headerBackgroundColor);
            }

            // Active checkbox
            activeField = GUI.Toggle(toggleRect, activeField, GUIContent.none, SmallTickbox);

            // Dropdown menu icon
            GUI.DrawTexture(menuRect, menuIcon);

            for (var i = 0; i < 3; i++)
            {
                var r = reorderRect;
                r.height = 1;
                r.y = reorderRect.y + reorderRect.height * (i / 3.0f);
                EditorGUI.DrawRect(r, Reorder);
            }


            // Handle events

            if (e.type == EventType.MouseDown)
                if (menuRect.Contains(e.mousePosition))
                {
                    var menu = new GenericMenu();
                    fillGenericMenu(menu);
                    menu.DropDown(new Rect(new Vector2(menuRect.x, menuRect.yMax), Vector2.zero));
                    e.Use();
                }

            if (e.type == EventType.MouseDown && labelRect.Contains(e.mousePosition) && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return backgroundRect;
        }
    }
}