using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you trigger a play on a target MMRadioSignal (usually used by a MMRadioBroadcaster to emit a
    ///     value that can then be listened to by MMRadioReceivers. From this feedback you can also specify a duration,
    ///     timescale and multiplier.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback will let you trigger a play on a target MMRadioSignal (usually used by a MMRadioBroadcaster to emit a value that can then be listened to by MMRadioReceivers. From this feedback you can also specify a duration, timescale and multiplier.")]
    [FeedbackPath("GameObject/MMRadioSignal")]
    public class MMF_RadioSignal : MMF_Feedback
    {
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float Duration = 1f;

        /// a global multiplier to apply to the end result of the combination
        [Tooltip("a global multiplier to apply to the end result of the combination")]
        public float GlobalMultiplier = 1f;

        [MMFInspectorGroup("Radio Signal", true, 72)]
        /// The target MMRadioSignal to trigger
        [Tooltip("The target MMRadioSignal to trigger")]
        public MMRadioSignal TargetSignal;

        /// the timescale to operate on
        [Tooltip("the timescale to operate on")]
        public MMRadioSignal.TimeScales TimeScale = MMRadioSignal.TimeScales.Unscaled;

        /// the duration of this feedback is the duration of the light, or 0 if instant
        public override float FeedbackDuration => 0f;


        /// <summary>
        ///     On Play we set the values on our target signal and make it start shaking its level
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (Active)
                if (TargetSignal != null)
                {
                    var intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;

                    TargetSignal.Duration = Duration;
                    TargetSignal.GlobalMultiplier = GlobalMultiplier * intensityMultiplier;
                    TargetSignal.TimeScale = TimeScale;
                    TargetSignal.StartShaking();
                }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            base.CustomStopFeedback(position, feedbacksIntensity);
            if (Active)
                if (TargetSignal != null)
                    TargetSignal.Stop();
        }

#if UNITY_EDITOR
        public override Color FeedbackColor => MMFeedbacksInspectorColors.GameObjectColor;

        public override bool EvaluateRequiresSetup()
        {
            return TargetSignal == null;
        }

        public override string RequiredTargetText => TargetSignal != null ? TargetSignal.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetSignal be set to be able to work properly. You can set one below.";
#endif
    }
}