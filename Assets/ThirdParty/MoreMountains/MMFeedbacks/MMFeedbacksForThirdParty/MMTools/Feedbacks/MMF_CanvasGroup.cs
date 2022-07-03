using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback lets you control the opacity of a canvas group over time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you control the opacity of a canvas group over time.")]
    [FeedbackPath("UI/CanvasGroup")]
    public class MMF_CanvasGroup : MMF_FeedbackBase
    {
        /// the curve to tween the opacity on
        [Tooltip("the curve to tween the opacity on")] [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public MMTweenType AlphaCurve =
            new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));

        /// the value to move the opacity to in instant mode
        [Tooltip("the value to move the opacity to in instant mode")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.Instant)]
        public float InstantAlpha;

        /// the value to remap the opacity curve's 1 to
        [Tooltip("the value to remap the opacity curve's 1 to")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public float RemapOne = 1f;

        /// the value to remap the opacity curve's 0 to
        [Tooltip("the value to remap the opacity curve's 0 to")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public float RemapZero = 0f;

        [MMFInspectorGroup("Canvas Group", true, 12, true)]
        /// the receiver to write the level to
        [Tooltip("the receiver to write the level to")]
        public CanvasGroup TargetCanvasGroup;

        protected override void FillTargets()
        {
            if (TargetCanvasGroup == null) return;

            var target = new MMF_FeedbackBaseTarget();
            var receiver = new MMPropertyReceiver();
            receiver.TargetObject = TargetCanvasGroup.gameObject;
            receiver.TargetComponent = TargetCanvasGroup;
            receiver.TargetPropertyName = "alpha";
            receiver.RelativeValue = RelativeValues;
            target.Target = receiver;
            target.LevelCurve = AlphaCurve;
            target.RemapLevelZero = RemapZero;
            target.RemapLevelOne = RemapOne;
            target.InstantLevel = InstantAlpha;

            _targets.Add(target);
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.UIColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetCanvasGroup == null;
        }

        public override string RequiredTargetText => TargetCanvasGroup != null ? TargetCanvasGroup.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetCanvasGroup be set to be able to work properly. You can set one below.";
#endif
    }
}