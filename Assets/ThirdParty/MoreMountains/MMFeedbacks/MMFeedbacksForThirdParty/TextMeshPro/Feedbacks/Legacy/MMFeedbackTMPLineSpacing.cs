﻿using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback lets you control the line spacing of a target TMP over time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you control the line spacing of a target TMP over time.")]
    [FeedbackPath("TextMesh Pro/TMP Line Spacing")]
    public class MMFeedbackTMPLineSpacing : MMFeedbackBase
    {
        [Header("Target")]
        /// the TMP_Text component to control
        [Tooltip("the TMP_Text component to control")]
        public TMP_Text TargetTMPText;

        [Header("Paragraph Spacing")]
        /// the curve to tween on
        [Tooltip("the curve to tween on")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public MMTweenType LineSpacingCurve =
            new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float RemapZero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")] [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float RemapOne = 10f;

        /// the value to move to in instant mode
        [Tooltip("the value to move to in instant mode")] [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantFontSize;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.TMPColor; }
        }
#endif

        protected override void FillTargets()
        {
            if (TargetTMPText == null) return;

            var target = new MMFeedbackBaseTarget();
            var receiver = new MMPropertyReceiver();
            receiver.TargetObject = TargetTMPText.gameObject;
            receiver.TargetComponent = TargetTMPText;
            receiver.TargetPropertyName = "lineSpacing";
            receiver.RelativeValue = RelativeValues;
            target.Target = receiver;
            target.LevelCurve = LineSpacingCurve;
            target.RemapLevelZero = RemapZero;
            target.RemapLevelOne = RemapOne;
            target.InstantLevel = InstantFontSize;

            _targets.Add(target);
        }
    }
}