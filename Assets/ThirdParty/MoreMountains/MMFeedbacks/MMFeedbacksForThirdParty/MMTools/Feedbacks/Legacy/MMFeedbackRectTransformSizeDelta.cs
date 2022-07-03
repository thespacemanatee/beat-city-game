using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback lets you control the size delta property (the size of this RectTransform relative to the distances
    ///     between the anchors) of a RectTransform, over time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback lets you control the size delta property (the size of this RectTransform relative to the distances between the anchors) of a RectTransform, over time")]
    [FeedbackPath("UI/RectTransformSizeDelta")]
    public class MMFeedbackRectTransformSizeDelta : MMFeedbackBase
    {
        [Header("Target")]
        /// the rect transform we want to impact
        [Tooltip("the rect transform we want to impact")]
        public RectTransform TargetRectTransform;

        [Header("Size Delta")]
        /// the speed at which we should animate the size delta
        [Tooltip("the speed at which we should animate the size delta")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public MMTweenType SpeedCurve = new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public Vector2 RemapZero = Vector2.zero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.Instant)]
        public Vector2 RemapOne = Vector2.one;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.UIColor; }
        }
#endif

        protected override void FillTargets()
        {
            if (TargetRectTransform == null) return;

            var target = new MMFeedbackBaseTarget();
            var receiver = new MMPropertyReceiver();
            receiver.TargetObject = TargetRectTransform.gameObject;
            receiver.TargetComponent = TargetRectTransform;
            receiver.TargetPropertyName = "sizeDelta";
            receiver.RelativeValue = RelativeValues;
            receiver.Vector2RemapZero = RemapZero;
            receiver.Vector2RemapOne = RemapOne;
            target.Target = receiver;
            target.LevelCurve = SpeedCurve;
            target.RemapLevelZero = 0f;
            target.RemapLevelOne = 1f;
            target.InstantLevel = 1f;

            _targets.Add(target);
        }
    }
}