using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    ///     This feedback allows you to control depth of field focus distance, aperture and focal length over time.
    ///     It requires you have in your scene an object with a PostProcessVolume
    ///     with Depth of Field active, and a MMDepthOfFieldShaker component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback allows you to control depth of field focus distance, aperture and focal length over time. " +
        "It requires you have in your scene an object with a PostProcessVolume " +
        "with Depth of Field active, and a MMDepthOfFieldShaker component.")]
    [FeedbackPath("PostProcess/Depth Of Field")]
    public class MMF_DepthOfField : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// whether or not to add to the initial values
        [Tooltip("whether or not to add to the initial values")]
        public bool RelativeValues = true;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")] [Range(0.1f, 32f)]
        public float RemapApertureOne = 0.2f;

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [Range(0.1f, 32f)]
        public float RemapApertureZero = 0.6f;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")] [Range(0f, 300f)]
        public float RemapFocalLengthOne = 27.5f;

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [Range(0f, 300f)]
        public float RemapFocalLengthZero = 27.5f;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        public float RemapFocusDistanceOne = 50f;

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        public float RemapFocusDistanceZero = 4f;

        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        public bool ResetShakerValuesAfterShake = true;

        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        public bool ResetTargetValuesAfterShake = true;

        [MMFInspectorGroup("Aperture", true, 53)]
        /// the curve used to animate the aperture value on
        [Tooltip("the curve used to animate the aperture value on")]
        public AnimationCurve ShakeAperture = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        [MMFInspectorGroup("Depth Of Field", true, 51)]
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float ShakeDuration = 2f;

        [MMFInspectorGroup("Focal Length", true, 54)]
        /// the curve used to animate the focal length value on
        [Tooltip("the curve used to animate the focal length value on")]
        public AnimationCurve ShakeFocalLength = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        [MMFInspectorGroup("Focus Distance", true, 52)]
        /// the curve used to animate the focus distance value on
        [Tooltip("the curve used to animate the focus distance value on")]
        public AnimationCurve ShakeFocusDistance = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration
        {
            get => ApplyTimeMultiplier(ShakeDuration);
            set => ShakeDuration = value;
        }

        public override bool HasChannel => true;

        /// <summary>
        ///     Triggers a DoF shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            var intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
            MMDepthOfFieldShakeEvent.Trigger(ShakeFocusDistance, FeedbackDuration, RemapFocusDistanceZero,
                RemapFocusDistanceOne,
                ShakeAperture, RemapApertureZero, RemapApertureOne,
                ShakeFocalLength, RemapFocalLengthZero, RemapFocalLengthOne,
                RelativeValues, intensityMultiplier, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake,
                NormalPlayDirection, Timing.TimescaleMode);
        }

        /// <summary>
        ///     On stop we stop our transition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized) return;
            base.CustomStopFeedback(position, feedbacksIntensity);

            MMDepthOfFieldShakeEvent.Trigger(ShakeFocusDistance, FeedbackDuration, RemapFocusDistanceZero,
                RemapFocusDistanceOne,
                ShakeAperture, RemapApertureZero, RemapApertureOne,
                ShakeFocalLength, RemapFocalLengthZero, RemapFocalLengthOne,
                RelativeValues, stop: true);
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.PostProcessColor; }
        }

        public override string RequiredTargetText => "Channel " + Channel;
#endif
    }
}