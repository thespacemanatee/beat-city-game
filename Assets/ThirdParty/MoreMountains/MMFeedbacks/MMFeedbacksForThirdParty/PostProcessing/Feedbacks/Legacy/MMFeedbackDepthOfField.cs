﻿using MoreMountains.Feedbacks;
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
    public class MMFeedbackDepthOfField : MMFeedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        [Header("Depth Of Field")]
        /// the channel to emit on
        [Tooltip("the channel to emit on")]
        public int Channel;

        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float ShakeDuration = 2f;

        /// whether or not to add to the initial values
        [Tooltip("whether or not to add to the initial values")]
        public bool RelativeValues = true;

        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        public bool ResetShakerValuesAfterShake = true;

        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        public bool ResetTargetValuesAfterShake = true;

        [Header("Focus Distance")]
        /// the curve used to animate the focus distance value on
        [Tooltip("the curve used to animate the focus distance value on")]
        public AnimationCurve ShakeFocusDistance = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        public float RemapFocusDistanceZero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        public float RemapFocusDistanceOne = 3f;

        [Header("Aperture")]
        /// the curve used to animate the aperture value on
        [Tooltip("the curve used to animate the aperture value on")]
        public AnimationCurve ShakeAperture = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [Range(0.1f, 32f)]
        public float RemapApertureZero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")] [Range(0.1f, 32f)]
        public float RemapApertureOne;

        [Header("Focal Length")]
        /// the curve used to animate the focal length value on
        [Tooltip("the curve used to animate the focal length value on")]
        public AnimationCurve ShakeFocalLength = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [Range(0f, 300f)]
        public float RemapFocalLengthZero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")] [Range(0f, 300f)]
        public float RemapFocalLengthOne;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.PostProcessColor; }
        }
#endif

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration
        {
            get => ApplyTimeMultiplier(ShakeDuration);
            set => ShakeDuration = value;
        }

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
    }
}