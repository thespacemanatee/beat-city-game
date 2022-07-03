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
    public class MMFeedbackCanvasGroup : MMFeedbackBase
    {
        [Header("Target")]
        /// the receiver to write the level to
        [Tooltip("the receiver to write the level to")]
        public CanvasGroup TargetCanvasGroup;

        [Header("Level")]
        /// the curve to tween the opacity on
        [Tooltip("the curve to tween the opacity on")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public MMTweenType AlphaCurve =
            new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));

        /// the value to remap the opacity curve's 0 to
        [Tooltip("the value to remap the opacity curve's 0 to")] [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float RemapZero;

        /// the value to remap the opacity curve's 1 to
        [Tooltip("the value to remap the opacity curve's 1 to")] [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float RemapOne = 1f;

        /// the value to move the opacity to in instant mode
        [Tooltip("the value to move the opacity to in instant mode")] [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantAlpha;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.UIColor; }
        }
#endif

        protected override void FillTargets()
        {
            if (TargetCanvasGroup == null) return;

            var target = new MMFeedbackBaseTarget();
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
    }
}