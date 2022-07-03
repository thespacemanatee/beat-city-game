using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     A custom editor displaying a foldable list of MMFeedbacks, a dropdown to add more, as well as test buttons to test
    ///     your feedbacks at runtime
    /// </summary>
    [CustomEditor(typeof(MMFeedbacks))]
    public class MMFeedbacksEditor : Editor
    {
        private static bool _debugView;
        private static bool _settingsMenuDropdown;
        private static bool _eventsMenuDropdown;
        protected bool _canDisplayInspector = true;
        protected GUIStyle _directionButtonStyle;
        protected int _draggedEndID = -1;
        protected int _draggedStartID = -1;

        protected Dictionary<MMFeedback, Editor> _editors;
        protected SerializedProperty _mmfeedbacks;
        protected SerializedProperty _mmfeedbacksAutoChangeDirectionOnEnd;
        protected SerializedProperty _mmfeedbacksAutoPlayOnEnable;
        protected SerializedProperty _mmfeedbacksAutoPlayOnStart;
        protected SerializedProperty _mmfeedbacksCanPlayWhileAlreadyPlaying;
        protected SerializedProperty _mmfeedbacksChanceToPlay;
        protected SerializedProperty _mmfeedbacksCooldownDuration;
        protected SerializedProperty _mmfeedbacksDirection;
        protected SerializedProperty _mmfeedbacksDisplayFullDurationDetails;
        protected SerializedProperty _mmfeedbacksDurationMultiplier;
        protected SerializedProperty _mmfeedbacksEvents;
        protected SerializedProperty _mmfeedbacksFeedbacksIntensity;
        protected SerializedProperty _mmfeedbacksInitialDelay;
        protected SerializedProperty _mmfeedbacksInitializationMode;
        protected SerializedProperty _mmfeedbacksSafeMode;
        protected Color _originalBackgroundColor;
        protected Color _playButtonColor = new Color32(193, 255, 2, 255);
        protected GUIStyle _playingStyle;
        protected Texture2D _scriptDrivenBoxBackgroundTexture;
        protected Color _scriptDrivenBoxColor;
        protected Color _scriptDrivenBoxColorFrom = new(1f, 0f, 0f, 1f);
        protected Color _scriptDrivenBoxColorTo = new(0.7f, 0.1f, 0.1f, 1f);

        protected MMFeedbacks _targetMMFeedbacks;
        protected string[] _typeDisplays;
        protected List<FeedbackTypePair> _typesAndNames = new();

        /// <summary>
        ///     On Enable, grabs properties and initializes the add feedback dropdown's contents
        /// </summary>
        private void OnEnable()
        {
            // Get properties
            _targetMMFeedbacks = target as MMFeedbacks;
            _mmfeedbacks = serializedObject.FindProperty("Feedbacks");
            _mmfeedbacksInitializationMode = serializedObject.FindProperty("InitializationMode");
            _mmfeedbacksSafeMode = serializedObject.FindProperty("SafeMode");
            _mmfeedbacksAutoPlayOnStart = serializedObject.FindProperty("AutoPlayOnStart");
            _mmfeedbacksAutoPlayOnEnable = serializedObject.FindProperty("AutoPlayOnEnable");
            _mmfeedbacksDirection = serializedObject.FindProperty("Direction");
            _mmfeedbacksAutoChangeDirectionOnEnd = serializedObject.FindProperty("AutoChangeDirectionOnEnd");
            _mmfeedbacksDurationMultiplier = serializedObject.FindProperty("DurationMultiplier");
            _mmfeedbacksDisplayFullDurationDetails = serializedObject.FindProperty("DisplayFullDurationDetails");
            _mmfeedbacksCooldownDuration = serializedObject.FindProperty("CooldownDuration");
            _mmfeedbacksInitialDelay = serializedObject.FindProperty("InitialDelay");
            _mmfeedbacksCanPlayWhileAlreadyPlaying = serializedObject.FindProperty("CanPlayWhileAlreadyPlaying");
            _mmfeedbacksFeedbacksIntensity = serializedObject.FindProperty("FeedbacksIntensity");
            _mmfeedbacksChanceToPlay = serializedObject.FindProperty("ChanceToPlay");

            _mmfeedbacksEvents = serializedObject.FindProperty("Events");

            // store GUI bg color
            _originalBackgroundColor = GUI.backgroundColor;

            // Repair routine to catch feedbacks that may have escaped due to Unity's serialization issues
            RepairRoutine();

            // Create editors
            _editors = new Dictionary<MMFeedback, Editor>();
            for (var i = 0; i < _mmfeedbacks.arraySize; i++)
                AddEditor(_mmfeedbacks.GetArrayElementAtIndex(i).objectReferenceValue as MMFeedback);

            // Retrieve available feedbacks
            var types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(MMFeedback))
                select assemblyType).ToList();

            // Create display list from types
            var typeNames = new List<string>();
            for (var i = 0; i < types.Count; i++)
            {
                var newType = new FeedbackTypePair();
                newType.FeedbackType = types[i];
                newType.FeedbackName = FeedbackPathAttribute.GetFeedbackDefaultPath(types[i]);
                if (newType.FeedbackName == "MMFeedbackBase") continue;
                _typesAndNames.Add(newType);
            }

            _typesAndNames = _typesAndNames.OrderBy(t => t.FeedbackName).ToList();

            typeNames.Add("Add new feedback...");
            for (var i = 0; i < _typesAndNames.Count; i++) typeNames.Add(_typesAndNames[i].FeedbackName);

            _typeDisplays = typeNames.ToArray();

            _directionButtonStyle = new GUIStyle();
            _directionButtonStyle.border.left = 0;
            _directionButtonStyle.border.right = 0;
            _directionButtonStyle.border.top = 0;
            _directionButtonStyle.border.bottom = 0;

            _playingStyle = new GUIStyle();
            _playingStyle.normal.textColor = Color.yellow;
        }

        /// <summary>
        ///     Calls the repair routine if needed
        /// </summary>
        protected virtual void RepairRoutine()
        {
            _targetMMFeedbacks = target as MMFeedbacks;
            if (_targetMMFeedbacks.SafeMode == MMFeedbacks.SafeModes.EditorOnly ||
                _targetMMFeedbacks.SafeMode == MMFeedbacks.SafeModes.Full) _targetMMFeedbacks.AutoRepair();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Draws the inspector, complete with helpbox, init mode selection, list of feedbacks, feedback selection and test
        ///     buttons
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (!_canDisplayInspector) return;

            var e = Event.current;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            if (!MMFeedbacks.GlobalMMFeedbacksActive)
            {
                var baseColor = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.HelpBox(
                    "All MMFeedbacks, including this one, are currently disabled. This is done via script, by changing the value of the MMFeedbacks.GlobalMMFeedbacksActive boolean. Right now this value has been set to false. Setting it back to true will allow MMFeedbacks to play again.",
                    MessageType.Warning);
                EditorGUILayout.Space();
                GUI.color = baseColor;
            }

            if (MMFeedbacksConfiguration.Instance.ShowInspectorTips)
                EditorGUILayout.HelpBox(
                    "The MMFeedbacks component is getting replaced by the new and improved MMF_Player, which will improve performance, let you keep runtime changes, and much more! The MMF_Player works just like MMFeedbacks. " +
                    "Consider using it instead, and don't hesitate to check the documentation to learn all about it.",
                    MessageType.Warning);

            var helpBoxRect = GUILayoutUtility.GetLastRect();

            if (EditorGUI.EndChangeCheck()) Undo.RecordObject(target, "Modified Feedback Manager");

            // Settings dropdown -------------------------------------------------------------------------------------

            _settingsMenuDropdown =
                EditorGUILayout.Foldout(_settingsMenuDropdown, "Settings", true, EditorStyles.foldout);
            if (_settingsMenuDropdown)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Initialization", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_mmfeedbacksSafeMode);
                EditorGUILayout.PropertyField(_mmfeedbacksInitializationMode);
                EditorGUILayout.PropertyField(_mmfeedbacksAutoPlayOnStart);
                EditorGUILayout.PropertyField(_mmfeedbacksAutoPlayOnEnable);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Direction", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_mmfeedbacksDirection);
                EditorGUILayout.PropertyField(_mmfeedbacksAutoChangeDirectionOnEnd);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Intensity", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_mmfeedbacksFeedbacksIntensity);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_mmfeedbacksDurationMultiplier);
                EditorGUILayout.PropertyField(_mmfeedbacksDisplayFullDurationDetails);
                EditorGUILayout.PropertyField(_mmfeedbacksCooldownDuration);
                EditorGUILayout.PropertyField(_mmfeedbacksInitialDelay);
                EditorGUILayout.PropertyField(_mmfeedbacksChanceToPlay);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Play Conditions", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_mmfeedbacksCanPlayWhileAlreadyPlaying);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_mmfeedbacksEvents);

                if (!Application.isPlaying)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Generate MMF_Player")) ConvertToMMF_Player(true);
                    if (GUILayout.Button("Convert to MMF_Player")) ConvertToMMF_Player(false);
                    EditorGUILayout.EndHorizontal();
                }
            }

            // Duration ----------------------------------------------------------------------------------------------

            var durationRectWidth = 70f;
            var durationRect = new Rect(helpBoxRect.xMax - durationRectWidth, helpBoxRect.yMax + 6, durationRectWidth,
                17f);
            durationRect.xMin = helpBoxRect.xMax - durationRectWidth;
            durationRect.xMax = helpBoxRect.xMax;

            var playingRectWidth = 70f;
            var playingRect = new Rect(helpBoxRect.xMax - playingRectWidth - durationRectWidth, helpBoxRect.yMax + 6,
                playingRectWidth, 17f);
            playingRect.xMin = helpBoxRect.xMax - durationRectWidth - playingRectWidth;
            playingRect.xMax = helpBoxRect.xMax;

            // Direction ----------------------------------------------------------------------------------------------

            var directionRectWidth = 16f;
            var directionRect = new Rect(helpBoxRect.xMax - directionRectWidth, helpBoxRect.yMax + 5,
                directionRectWidth, 17f);
            directionRect.xMin = helpBoxRect.xMax - directionRectWidth;
            directionRect.xMax = helpBoxRect.xMax;

            if ((target as MMFeedbacks).IsPlaying) GUI.Label(playingRect, "[PLAYING] ", _playingStyle);

            GUI.Label(durationRect, "[" + _targetMMFeedbacks.TotalDuration.ToString("F2") + "s]");

            if (_targetMMFeedbacks.Direction == MMFeedbacks.Directions.BottomToTop)
            {
                var arrowUpIcon = Resources.Load("FeelArrowUp") as Texture;
                var directionIcon = new GUIContent(arrowUpIcon);

                if (GUI.Button(directionRect, directionIcon, _directionButtonStyle)) _targetMMFeedbacks.Revert();
            }
            else
            {
                var arrowDownIcon = Resources.Load("FeelArrowDown") as Texture;
                var directionIcon = new GUIContent(arrowDownIcon);

                if (GUI.Button(directionRect, directionIcon, _directionButtonStyle)) _targetMMFeedbacks.Revert();
            }

            // Draw list ------------------------------------------------------------------------------------------

            MMFeedbackStyling.DrawSection("Feedbacks");

            if (!_canDisplayInspector) return;

            for (var i = 0; i < _mmfeedbacks.arraySize; i++)
            {
                if (!_canDisplayInspector) return;

                MMFeedbackStyling.DrawSplitter();

                var property = _mmfeedbacks.GetArrayElementAtIndex(i);

                // Failsafe but should not happen
                if (property.objectReferenceValue == null) continue;

                // Retrieve feedback

                var feedback = property.objectReferenceValue as MMFeedback;
                feedback.hideFlags = _debugView ? HideFlags.None : HideFlags.HideInInspector;

                Undo.RecordObject(feedback, "Modified Feedback");

                // Draw header

                var id = i;
                var isExpanded = property.isExpanded;
                var label = feedback.Label;
                var pause = false;

                if (feedback.Pause != null) pause = true;
                if (feedback.LooperPause && Application.isPlaying)
                {
                    if ((feedback as MMFeedbackLooper).InfiniteLoop)
                        label = label + "[Infinite Loop] ";
                    else
                        label = label + "[ " + (feedback as MMFeedbackLooper).NumberOfLoopsLeft + " loops left ] ";
                }

                var headerRect = MMFeedbackStyling.DrawHeader(
                    ref isExpanded,
                    ref feedback.Active,
                    label,
                    feedback.FeedbackColor,
                    menu =>
                    {
                        if (Application.isPlaying)
                            menu.AddItem(new GUIContent("Play"), false, () => PlayFeedback(id));
                        else
                            menu.AddDisabledItem(new GUIContent("Play"));
                        menu.AddSeparator(null);
                        //menu.AddItem(new GUIContent("Reset"), false, () => ResetFeedback(id));
                        menu.AddItem(new GUIContent("Remove"), false, () => RemoveFeedback(id));
                        menu.AddSeparator(null);
                        menu.AddItem(new GUIContent("Copy"), false, () => CopyFeedback(id));
                        if (FeedbackCopy.HasCopy() && FeedbackCopy.Type == feedback.GetType())
                            menu.AddItem(new GUIContent("Paste"), false, () => PasteFeedback(id));
                        else
                            menu.AddDisabledItem(new GUIContent("Paste"));
                    },
                    feedback.FeedbackStartedAt,
                    feedback.FeedbackDuration,
                    feedback.TotalDuration,
                    feedback.Timing,
                    pause,
                    _targetMMFeedbacks
                );

                // Check if we start dragging this feedback

                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (headerRect.Contains(e.mousePosition))
                        {
                            _draggedStartID = i;
                            e.Use();
                        }

                        break;
                }

                // Draw blue rect if feedback is being dragged

                if (_draggedStartID == i && headerRect != Rect.zero)
                {
                    var color = new Color(0, 1, 1, 0.2f);
                    EditorGUI.DrawRect(headerRect, color);
                }

                // If hovering at the top of the feedback while dragging one, check where the feedback should be dropped : top or bottom

                if (headerRect.Contains(e.mousePosition))
                    if (_draggedStartID >= 0)
                    {
                        _draggedEndID = i;

                        var headerSplit = headerRect;
                        headerSplit.height *= 0.5f;
                        headerSplit.y += headerSplit.height;
                        if (headerSplit.Contains(e.mousePosition))
                            _draggedEndID = i + 1;
                    }

                // If expanded, draw feedback editor

                property.isExpanded = isExpanded;
                if (isExpanded)
                {
                    EditorGUI.BeginDisabledGroup(!feedback.Active);

                    var helpText = FeedbackHelpAttribute.GetFeedbackHelpText(feedback.GetType());

                    if (!string.IsNullOrEmpty(helpText) && MMFeedbacksConfiguration.Instance.ShowInspectorTips)
                    {
                        var style = new GUIStyle(EditorStyles.helpBox);
                        style.richText = true;
                        var newHeight = style.CalcHeight(new GUIContent(helpText), EditorGUIUtility.currentViewWidth);
                        EditorGUILayout.LabelField(helpText, style);
                    }

                    EditorGUILayout.Space();

                    if (!_editors.ContainsKey(feedback)) AddEditor(feedback);

                    var editor = _editors[feedback];
                    CreateCachedEditor(feedback, feedback.GetType(), ref editor);

                    editor.OnInspectorGUI();

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Play", EditorStyles.miniButtonMid)) PlayFeedback(id);
                        if (GUILayout.Button("Stop", EditorStyles.miniButtonMid)) StopFeedback(id);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }

            // Draw add new item

            if (_mmfeedbacks.arraySize > 0) MMFeedbackStyling.DrawSplitter();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                // Feedback list

                var newItem = EditorGUILayout.Popup(0, _typeDisplays) - 1;
                if (newItem >= 0) AddFeedback(_typesAndNames[newItem].FeedbackType);

                // Paste feedback copy as new

                if (FeedbackCopy.HasCopy())
                    if (GUILayout.Button("Paste as new", EditorStyles.miniButton,
                            GUILayout.Width(EditorStyles.miniButton.CalcSize(new GUIContent("Paste as new")).x)))
                        PasteAsNew();

                if (FeedbackCopy.HasMultipleCopies())
                    if (GUILayout.Button("Paste all as new", EditorStyles.miniButton,
                            GUILayout.Width(EditorStyles.miniButton.CalcSize(new GUIContent("Paste all as new")).x)))
                        PasteAllAsNew();
            }

            if (!FeedbackCopy.HasMultipleCopies())
                if (GUILayout.Button("Copy all", EditorStyles.miniButton,
                        GUILayout.Width(EditorStyles.miniButton.CalcSize(new GUIContent("Paste as new")).x)))
                    CopyAll();

            EditorGUILayout.EndHorizontal();

            // Reorder

            if (_draggedStartID >= 0 && _draggedEndID >= 0)
                if (_draggedEndID != _draggedStartID)
                {
                    if (_draggedEndID > _draggedStartID)
                        _draggedEndID--;
                    _mmfeedbacks.MoveArrayElement(_draggedStartID, _draggedEndID);
                    _draggedStartID = _draggedEndID;
                }

            if (_draggedStartID >= 0 || _draggedEndID >= 0)
                switch (e.type)
                {
                    case EventType.MouseUp:
                        _draggedStartID = -1;
                        _draggedEndID = -1;
                        e.Use();
                        break;
                }

            // Clean up

            var wasRemoved = false;
            for (var i = _mmfeedbacks.arraySize - 1; i >= 0; i--)
                if (_mmfeedbacks.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    wasRemoved = true;
                    _mmfeedbacks.DeleteArrayElementAtIndex(i);
                }

            if (wasRemoved)
            {
                var gameObject = (target as MMFeedbacks).gameObject;
                foreach (var c in gameObject.GetComponents<Component>())
                    if (c != null)
                        c.hideFlags = HideFlags.None;
            }

            // Apply changes

            serializedObject.ApplyModifiedProperties();

            // Draw debug


            MMFeedbackStyling.DrawSection("All Feedbacks Debug");

            // Testing buttons

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            EditorGUILayout.BeginHorizontal();
            {
                // initialize button
                if (GUILayout.Button("Initialize", EditorStyles.miniButtonLeft))
                    (target as MMFeedbacks).Initialization();

                // play button
                _originalBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = _playButtonColor;
                if (GUILayout.Button("Play", EditorStyles.miniButtonMid)) (target as MMFeedbacks).PlayFeedbacks();
                GUI.backgroundColor = _originalBackgroundColor;

                // pause button
                if ((target as MMFeedbacks).ContainsLoop)
                    if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
                        (target as MMFeedbacks).PauseFeedbacks();

                // stop button
                if (GUILayout.Button("Stop", EditorStyles.miniButtonMid)) (target as MMFeedbacks).StopFeedbacks();

                // reset button
                if (GUILayout.Button("Reset", EditorStyles.miniButtonMid)) (target as MMFeedbacks).ResetFeedbacks();
                EditorGUI.EndDisabledGroup();

                // reverse button
                if (GUILayout.Button("Revert", EditorStyles.miniButtonMid)) (target as MMFeedbacks).Revert();

                // debug button
                EditorGUI.BeginChangeCheck();
                {
                    _debugView = GUILayout.Toggle(_debugView, "Debug View", EditorStyles.miniButtonRight);

                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (var f in (target as MMFeedbacks).Feedbacks)
                            f.hideFlags = _debugView ? HideFlags.HideInInspector : HideFlags.None;
                        InternalEditorUtility.RepaintAllViews();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();


            var pingPong = Mathf.PingPong(Time.unscaledTime, 0.25f);

            // if in pause, we display additional controls
            if (_targetMMFeedbacks.InScriptDrivenPause)
            {
                // draws a warning box
                _scriptDrivenBoxColor = Color.Lerp(_scriptDrivenBoxColorFrom, _scriptDrivenBoxColorTo, pingPong);
                GUI.skin.box.normal.background = Texture2D.whiteTexture;
                GUI.backgroundColor = _scriptDrivenBoxColor;
                GUI.skin.box.normal.textColor = Color.black;
                GUILayout.Box("Script driven pause in progress, call Resume() to exit pause",
                    GUILayout.ExpandWidth(true));
                GUI.backgroundColor = _originalBackgroundColor;
                GUI.skin.box.normal.background = _scriptDrivenBoxBackgroundTexture;

                // draws resume button
                if (GUILayout.Button("Resume")) _targetMMFeedbacks.ResumeFeedbacks();
            }

            // Debug draw
            if (_debugView)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_mmfeedbacks, true);
                EditorGUI.EndDisabledGroup();
            }
        }

        /// <summary>
        ///     We need to repaint constantly if dragging a feedback around
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        /// <summary>
        ///     Add a feedback to the list
        /// </summary>
        protected virtual MMFeedback AddFeedback(Type type)
        {
            /*GameObject gameObject = (target as MMFeedbacks).gameObject;

            MMFeedback newFeedback = Undo.AddComponent(gameObject, type) as MMFeedback;
            newFeedback.hideFlags = _debugView ? HideFlags.None : HideFlags.HideInInspector;
            newFeedback.Label = FeedbackPathAttribute.GetFeedbackDefaultName(type);

            AddEditor(newFeedback);

            _mmfeedbacks.arraySize++;
            _mmfeedbacks.GetArrayElementAtIndex(_mmfeedbacks.arraySize - 1).objectReferenceValue = newFeedback;

            return newFeedback;*/
            return (target as MMFeedbacks).AddFeedback(type);
        }

        /// <summary>
        ///     Remove the selected feedback
        /// </summary>
        protected virtual void RemoveFeedback(int id)
        {
            /*SerializedProperty property = _mmfeedbacks.GetArrayElementAtIndex(id);
            MMFeedback feedback = property.objectReferenceValue as MMFeedback;

            (target as MMFeedbacks).Feedbacks.Remove(feedback);

            _editors.Remove(feedback);
            Undo.DestroyObjectImmediate(feedback);*/

            (target as MMFeedbacks).RemoveFeedback(id);
        }

        //
        // Editors management
        //

        /// <summary>
        ///     Create the editor for a feedback
        /// </summary>
        protected virtual void AddEditor(MMFeedback feedback)
        {
            if (feedback == null)
                return;

            if (!_editors.ContainsKey(feedback))
            {
                Editor editor = null;
                CreateCachedEditor(feedback, null, ref editor);

                _editors.Add(feedback, editor);
            }
        }

        /// <summary>
        ///     Destroy the editor for a feedback
        /// </summary>
        protected virtual void RemoveEditor(MMFeedback feedback)
        {
            if (feedback == null)
                return;

            if (_editors.ContainsKey(feedback))
            {
                DestroyImmediate(_editors[feedback]);
                _editors.Remove(feedback);
            }
        }

        //
        // Feedback generic menus
        //

        /// <summary>
        ///     Play the selected feedback
        /// </summary>
        protected virtual void InitializeFeedback(int id)
        {
            var property = _mmfeedbacks.GetArrayElementAtIndex(id);
            var feedback = property.objectReferenceValue as MMFeedback;
            feedback.Initialization(feedback.gameObject);
        }

        /// <summary>
        ///     Play the selected feedback
        /// </summary>
        protected virtual void PlayFeedback(int id)
        {
            var property = _mmfeedbacks.GetArrayElementAtIndex(id);
            var feedback = property.objectReferenceValue as MMFeedback;
            feedback.Play(feedback.transform.position, _targetMMFeedbacks.FeedbacksIntensity);
        }

        /// <summary>
        ///     Play the selected feedback
        /// </summary>
        protected virtual void StopFeedback(int id)
        {
            var property = _mmfeedbacks.GetArrayElementAtIndex(id);
            var feedback = property.objectReferenceValue as MMFeedback;
            feedback.Stop(feedback.transform.position);
        }

        /// <summary>
        ///     Resets the selected feedback
        /// </summary>
        /// <param name="id"></param>
        protected virtual void ResetFeedback(int id)
        {
            var property = _mmfeedbacks.GetArrayElementAtIndex(id);
            var feedback = property.objectReferenceValue as MMFeedback;
            feedback.ResetFeedback();
        }

        /// <summary>
        ///     Copy the selected feedback
        /// </summary>
        protected virtual void CopyFeedback(int id)
        {
            var property = _mmfeedbacks.GetArrayElementAtIndex(id);
            var feedback = property.objectReferenceValue as MMFeedback;

            FeedbackCopy.Copy(new SerializedObject(feedback));
        }

        /// <summary>
        ///     Asks for a full copy of the source
        /// </summary>
        protected virtual void CopyAll()
        {
            FeedbackCopy.CopyAll(target as MMFeedbacks);
        }

        /// <summary>
        ///     Paste the previously copied feedback values into the selected feedback
        /// </summary>
        protected virtual void PasteFeedback(int id)
        {
            var property = _mmfeedbacks.GetArrayElementAtIndex(id);
            var feedback = property.objectReferenceValue as MMFeedback;

            var serialized = new SerializedObject(feedback);

            FeedbackCopy.Paste(serialized);
            serialized.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Creates a new feedback and applies the previoulsy copied feedback values
        /// </summary>
        protected virtual void PasteAsNew()
        {
            var newFeedback = AddFeedback(FeedbackCopy.Type);
            var serialized = new SerializedObject(newFeedback);

            serialized.Update();
            FeedbackCopy.Paste(serialized);
            serialized.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Asks for a paste of all feedbacks in the source
        /// </summary>
        protected virtual void PasteAllAsNew()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "Paste all MMFeedbacks");
            FeedbackCopy.PasteAll(this);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Converts a MMFeedbacks and all its contents into the new and improved MMF_Player
        ///     To convert from MMFeedbacks to MMF_Player you have a few options :
        ///     - on a MMFeedbacks Settings panel, press the Generate MMF_Player button, this will create a new MMF_Player
        ///     component on the same object, and copy feedbacks to it, ready to use.
        ///     The old MMFeedbacks won't be touched.
        ///     - press the Convert feedback, and this will try to replace the current MMFeedbacks with a new MMF_Player with the
        ///     same settings.
        ///     - if you're inside a prefab instance, regular replace won't work, and you'll want to add, at the prefab level, an
        ///     empty MMF_Player that will be the recipient of the conversion
        /// </summary>
        public virtual void ConvertToMMF_Player(bool generateOnly)
        {
            var targetObject = _targetMMFeedbacks.gameObject;
            var oldMMFPlayer = targetObject.GetComponent<MMF_Player>();

            // we remove any MMF_Player that may already be on that object
            if (oldMMFPlayer != null && oldMMFPlayer.FeedbacksList.Count > 0) DestroyImmediate(oldMMFPlayer);

            // if we don't have an old player to work with and are inside a prefab, we give up to not break stuff
            if (!generateOnly && oldMMFPlayer == null)
                // conversion can't happen on a prefab instance unfortunately 
                if (PrefabUtility.IsPartOfPrefabAsset(targetObject)
                    || PrefabUtility.IsPartOfPrefabInstance(targetObject)
                    || PrefabUtility.IsPartOfNonAssetPrefabInstance(targetObject))
                {
                    Debug.LogWarning("Unfortunately, you can't use conversion on a prefab instance.");
                    return;
                }

            _canDisplayInspector = false;
            serializedObject.Update();
            Undo.RegisterCompleteObjectUndo(target, "Convert to MMF_Player");
            Debug.Log("Starting conversion to MMF_Player --------");

            if (generateOnly)
            {
                // we create a new player
                if (oldMMFPlayer == null)
                {
                    var newPlayer = targetObject.AddComponent<MMF_Player>();
                    CopyFromMMFeedbacksToMMF_Player(newPlayer);
                }
                else
                {
                    CopyFromMMFeedbacksToMMF_Player(oldMMFPlayer);
                }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            GameObject temporaryHost = null;
            // we create a new player
            if (oldMMFPlayer == null)
            {
                temporaryHost = new GameObject("TemporaryHost");
                var newPlayer = temporaryHost.AddComponent<MMF_Player>();
                CopyFromMMFeedbacksToMMF_Player(newPlayer);

                var yourReplacementScript = MonoScript.FromMonoBehaviour(newPlayer);
                var scriptProperty = serializedObject.FindProperty("m_Script");
                serializedObject.Update();
                scriptProperty.objectReferenceValue = yourReplacementScript;
                serializedObject.ApplyModifiedProperties();

                // we copy back from our temp object
                var finalPlayer = targetObject.GetComponent<MMF_Player>();
                finalPlayer.InitializationMode = newPlayer.InitializationMode;
                finalPlayer.SafeMode = newPlayer.SafeMode;
                finalPlayer.Direction = newPlayer.Direction;
                finalPlayer.AutoChangeDirectionOnEnd = newPlayer.AutoChangeDirectionOnEnd;
                finalPlayer.AutoPlayOnStart = newPlayer.AutoPlayOnStart;
                finalPlayer.AutoPlayOnEnable = newPlayer.AutoPlayOnEnable;
                finalPlayer.DurationMultiplier = newPlayer.DurationMultiplier;
                finalPlayer.DisplayFullDurationDetails = newPlayer.DisplayFullDurationDetails;
                finalPlayer.CooldownDuration = newPlayer.CooldownDuration;
                finalPlayer.InitialDelay = newPlayer.InitialDelay;
                finalPlayer.CanPlayWhileAlreadyPlaying = newPlayer.CanPlayWhileAlreadyPlaying;
                finalPlayer.FeedbacksIntensity = newPlayer.FeedbacksIntensity;
                finalPlayer.Events = newPlayer.Events;
                finalPlayer.FeedbacksList = newPlayer.FeedbacksList;
                if (finalPlayer.FeedbacksList != null && finalPlayer.FeedbacksList.Count > 0)
                    foreach (var feedback in finalPlayer.FeedbacksList)
                    {
                        feedback.Owner = finalPlayer;
                        feedback.UniqueID = Guid.NewGuid().GetHashCode();
                    }
            }
            else
            {
                CopyFromMMFeedbacksToMMF_Player(oldMMFPlayer);
                PrefabUtility.RecordPrefabInstancePropertyModifications(oldMMFPlayer);
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                DestroyImmediate(_targetMMFeedbacks);
            }

            // we remove all remaining feedbacks
            var feedbackArray = targetObject.GetComponents(typeof(MMFeedback));
            foreach (var comp in feedbackArray) DestroyImmediate(comp);

            if (temporaryHost != null) DestroyImmediate(temporaryHost);

            Debug.Log("Conversion complete --------");
        }

        protected virtual void CopyFromMMFeedbacksToMMF_Player(MMF_Player newPlayer)
        {
            // we copy all its settings
            newPlayer.InitializationMode = _targetMMFeedbacks.InitializationMode;
            newPlayer.SafeMode = _targetMMFeedbacks.SafeMode;
            newPlayer.Direction = _targetMMFeedbacks.Direction;
            newPlayer.AutoChangeDirectionOnEnd = _targetMMFeedbacks.AutoChangeDirectionOnEnd;
            newPlayer.AutoPlayOnStart = _targetMMFeedbacks.AutoPlayOnStart;
            newPlayer.AutoPlayOnEnable = _targetMMFeedbacks.AutoPlayOnEnable;
            newPlayer.DurationMultiplier = _targetMMFeedbacks.DurationMultiplier;
            newPlayer.DisplayFullDurationDetails = _targetMMFeedbacks.DisplayFullDurationDetails;
            newPlayer.CooldownDuration = _targetMMFeedbacks.CooldownDuration;
            newPlayer.InitialDelay = _targetMMFeedbacks.InitialDelay;
            newPlayer.CanPlayWhileAlreadyPlaying = _targetMMFeedbacks.CanPlayWhileAlreadyPlaying;
            newPlayer.FeedbacksIntensity = _targetMMFeedbacks.FeedbacksIntensity;
            newPlayer.Events = _targetMMFeedbacks.Events;

            // we copy all its feedbacks
            var feedbacks = serializedObject.FindProperty("Feedbacks");
            for (var i = 0; i < feedbacks.arraySize; i++)
            {
                var oldFeedback = feedbacks.GetArrayElementAtIndex(i).objectReferenceValue as MMFeedback;

                // we look for a match in the new classes
                var oldType = oldFeedback.GetType();
                var oldTypeName = oldType.Name;
                var newTypeName = oldTypeName.Replace("MMFeedback", "MMF_");
                var newType = MMFeedbackStaticMethods.MMFGetTypeByName(newTypeName);

                if (newType == null)
                {
                    Debug.Log("<color=red>Couldn't find any MMF_Feedback matching " + oldTypeName + "</color>");
                }
                else
                {
                    var newFeedback = newPlayer.AddFeedback(newType);

                    List<FieldInfo> oldFieldsList;
                    var oldFieldsListLength = MMF_FieldInfo.GetFieldInfo(oldFeedback, out oldFieldsList);

                    for (var j = 0; j < oldFieldsListLength; j++)
                    {
                        var searchedField = oldFieldsList[j].Name;

                        if (!FeedbackCopy.IgnoreList.Contains(searchedField))
                        {
                            var newField = newType.GetField(searchedField);
                            var oldField = oldType.GetField(searchedField);

                            if (newField != null)
                            {
                                if (newField.FieldType == oldField.FieldType)
                                {
                                    newField.SetValue(newFeedback, oldField.GetValue(oldFeedback));
                                }
                                else
                                {
                                    if (oldField.FieldType.IsEnum)
                                        newField.SetValue(newFeedback, (int)oldField.GetValue(oldFeedback));
                                }
                            }
                        }
                    }

                    Debug.Log("Added new feedback of type " + newTypeName);
                }
            }

            newPlayer.RefreshCache();
        }

        /// <summary>
        ///     A data structure to store types and names
        /// </summary>
        public class FeedbackTypePair
        {
            public string FeedbackName;
            public Type FeedbackType;
        }

        /// <summary>
        ///     A helper class to copy and paste feedback properties
        /// </summary>
        private static class FeedbackCopy
        {
            private static readonly List<SerializedProperty> Properties = new();

            public static readonly string[] IgnoreList =
            {
                "m_ObjectHideFlags",
                "m_CorrespondingSourceObject",
                "m_PrefabInstance",
                "m_PrefabAsset",
                "m_GameObject",
                "m_Enabled",
                "m_EditorHideFlags",
                "m_Script",
                "m_Name",
                "m_EditorClassIdentifier"
            };
            // Single Copy --------------------------------------------------------------------

            public static Type Type { get; private set; }

            public static void Copy(SerializedObject serializedObject)
            {
                Type = serializedObject.targetObject.GetType();
                Properties.Clear();

                var property = serializedObject.GetIterator();
                property.Next(true);
                do
                {
                    if (!IgnoreList.Contains(property.name)) Properties.Add(property.Copy());
                } while (property.Next(false));
            }

            public static void Paste(SerializedObject target)
            {
                if (target.targetObject.GetType() == Type)
                    for (var i = 0; i < Properties.Count; i++)
                        target.CopyFromSerializedProperty(Properties[i]);
            }

            public static bool HasCopy()
            {
                return Properties != null && Properties.Count > 0;
            }

            // Multiple Copy ----------------------------------------------------------

            public static void CopyAll(MMFeedbacks sourceFeedbacks)
            {
                MMFeedbacksConfiguration.Instance._mmFeedbacks = sourceFeedbacks;
            }

            public static bool HasMultipleCopies()
            {
                return MMFeedbacksConfiguration.Instance._mmFeedbacks != null;
            }

            public static void PasteAll(MMFeedbacksEditor targetEditor)
            {
                var sourceFeedbacks = new SerializedObject(MMFeedbacksConfiguration.Instance._mmFeedbacks);
                var feedbacks = sourceFeedbacks.FindProperty("Feedbacks");

                for (var i = 0; i < feedbacks.arraySize; i++)
                {
                    var arrayFeedback = feedbacks.GetArrayElementAtIndex(i).objectReferenceValue as MMFeedback;

                    Copy(new SerializedObject(arrayFeedback));
                    var newFeedback = targetEditor.AddFeedback(arrayFeedback.GetType());
                    var serialized = new SerializedObject(newFeedback);
                    serialized.Update();
                    Paste(serialized);
                    serialized.ApplyModifiedProperties();
                }

                MMFeedbacksConfiguration.Instance._mmFeedbacks = null;
            }
        }
    }
}