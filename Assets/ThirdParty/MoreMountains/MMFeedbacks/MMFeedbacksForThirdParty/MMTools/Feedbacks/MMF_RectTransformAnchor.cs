using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback lets you control the min and max anchors of a RectTransform over time. That's the normalized position
    ///     in the parent RectTransform that the lower left and upper right corners are anchored to.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback lets you control the min and max anchors of a RectTransform over time. That's the normalized position in the parent RectTransform that the lower left and upper right corners are anchored to.")]
    [FeedbackPath("UI/RectTransform Anchor")]
    public class MMF_RectTransformAnchor : MMF_FeedbackBase
    {
        /// the curve to animate the max anchor on
        [Tooltip("the curve to animate the max anchor on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public MMTweenType AnchorMaxCurve = new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));

        /// the value to remap the max anchor curve's 1 on
        [Tooltip("the value to remap the max anchor curve's 1 on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime, (int)MMFeedbackBase.Modes.Instant)]
        public Vector2 AnchorMaxRemapOne = Vector2.one;

        /// the value to remap the max anchor curve's 0 on
        [Tooltip("the value to remap the max anchor curve's 0 on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public Vector2 AnchorMaxRemapZero = Vector2.zero;

        /// the curve to animate the min anchor on
        [Tooltip("the curve to animate the min anchor on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public MMTweenType AnchorMinCurve = new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));

        /// the value to remap the min anchor curve's 1 on
        [Tooltip("the value to remap the min anchor curve's 1 on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime, (int)MMFeedbackBase.Modes.Instant)]
        public Vector2 AnchorMinRemapOne = Vector2.one;

        /// the value to remap the min anchor curve's 0 on
        [Tooltip("the value to remap the min anchor curve's 0 on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public Vector2 AnchorMinRemapZero = Vector2.zero;

        [MMFInspectorGroup("Anchor Max", true, 44)]
        /// whether or not to modify the max anchor
        [Tooltip("whether or not to modify the max anchor")]
        public bool ModifyAnchorMax = true;

        [MMFInspectorGroup("Anchor Min", true, 43)]
        /// whether or not to modify the min anchor
        [Tooltip("whether or not to modify the min anchor")]
        public bool ModifyAnchorMin = true;

        [MMFInspectorGroup("Target RectTransform", true, 37, true)]
        /// the target RectTransform to control
        [Tooltip("the target RectTransform to control")]
        public RectTransform TargetRectTransform;

        protected override void FillTargets()
        {
            if (TargetRectTransform == null) return;

            var targetMin = new MMF_FeedbackBaseTarget();
            var receiverMin = new MMPropertyReceiver();
            receiverMin.TargetObject = TargetRectTransform.gameObject;
            receiverMin.TargetComponent = TargetRectTransform;
            receiverMin.TargetPropertyName = "anchorMin";
            receiverMin.RelativeValue = RelativeValues;
            receiverMin.Vector2RemapZero = AnchorMinRemapZero;
            receiverMin.Vector2RemapOne = AnchorMinRemapOne;
            receiverMin.ShouldModifyValue = ModifyAnchorMin;
            targetMin.Target = receiverMin;
            targetMin.LevelCurve = AnchorMinCurve;
            targetMin.RemapLevelZero = 0f;
            targetMin.RemapLevelOne = 1f;
            targetMin.InstantLevel = 1f;

            _targets.Add(targetMin);

            var targetMax = new MMF_FeedbackBaseTarget();
            var receiverMax = new MMPropertyReceiver();
            receiverMax.TargetObject = TargetRectTransform.gameObject;
            receiverMax.TargetComponent = TargetRectTransform;
            receiverMax.TargetPropertyName = "anchorMax";
            receiverMax.RelativeValue = RelativeValues;
            receiverMax.Vector2RemapZero = AnchorMaxRemapZero;
            receiverMax.Vector2RemapOne = AnchorMaxRemapOne;
            receiverMax.ShouldModifyValue = ModifyAnchorMax;
            targetMax.Target = receiverMax;
            targetMax.LevelCurve = AnchorMaxCurve;
            targetMax.RemapLevelZero = 0f;
            targetMax.RemapLevelOne = 1f;
            targetMax.InstantLevel = 1f;

            _targets.Add(targetMax);
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.UIColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetRectTransform == null;
        }

        public override string RequiredTargetText => TargetRectTransform != null ? TargetRectTransform.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetRectTransform be set to be able to work properly. You can set one below.";
#endif
    }
}