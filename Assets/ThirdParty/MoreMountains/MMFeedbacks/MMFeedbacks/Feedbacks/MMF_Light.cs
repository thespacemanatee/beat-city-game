﻿using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you control the color and intensity of a Light when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback lets you control the color and intensity of a Light in your scene for a certain duration (or instantly).")]
    [FeedbackPath("Light")]
    public class MMF_Light : MMF_Feedback
    {
        /// the possible modes for this feedback
        public enum Modes
        {
            OverTime,
            Instant,
            ShakerEvent
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected Coroutine _coroutine;
        protected float _initialIntensity;

        protected float _initialRange;
        protected float _initialShadowStrength;

        /// if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over
        [Tooltip(
            "if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
        public bool AllowAdditivePlays = false;

        [MMFInspectorGroup("Light", true, 37, true)]
        /// the light to affect when playing the feedback
        [Tooltip("the light to affect when playing the feedback")]
        public Light BoundLight;

        /// the colors to apply to the light over time
        [Tooltip("the colors to apply to the light over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public Gradient ColorOverTime;

        /// if this is true, the light will be disabled when this feedbacks is stopped
        [Tooltip("if this is true, the light will be disabled when this feedbacks is stopped")]
        public bool DisableOnStop = true;

        /// how long the light should change over time
        [Tooltip("how long the light should change over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float Duration = 0.2f;

        /// the transform to use to broadcast the event as origin point
        [Tooltip("the transform to use to broadcast the event as origin point")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public Transform EventOriginTransform;

        /// the range of the event, in units
        [Tooltip("the range of the event, in units")] [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public float EventRange = 100f;

        /// the color to move to in instant mode
        [Tooltip("the color to move to in instant mode")]
        [MMFEnumCondition("Mode", (int)Modes.Instant, (int)Modes.ShakerEvent)]
        public Color InstantColor;

        /// the value to move the intensity to in instant mode
        [Tooltip("the value to move the intensity to in instant mode")] [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantIntensity;

        /// the value to move the intensity to in instant mode
        [Tooltip("the value to move the intensity to in instant mode")] [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantRange;

        /// the value to move the shadow strength to in instant mode
        [Tooltip("the value to move the shadow strength to in instant mode")]
        [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantShadowStrength;

        [Header("Intensity")]
        /// the curve to tween the intensity on
        [Tooltip("the curve to tween the intensity on")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public AnimationCurve IntensityCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));

        /// whether the feedback should affect the light instantly or over a period of time
        [Tooltip("whether the feedback should affect the light instantly or over a period of time")]
        public Modes Mode = Modes.OverTime;

        [Header("Color")]
        /// whether or not to modify the color of the light
        [Tooltip("whether or not to modify the color of the light")]
        public bool ModifyColor = true;

        [Header("Range")]
        /// the range to apply to the light over time
        [Tooltip("the range to apply to the light over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public AnimationCurve RangeCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));

        /// whether or not the values should be relative or not
        [Tooltip("whether or not the values should be relative or not")]
        public bool RelativeValues = true;

        /// the value to remap the intensity curve's 1 to
        [Tooltip("the value to remap the intensity curve's 1 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapIntensityOne = 1f;

        /// the value to remap the intensity curve's 0 to
        [Tooltip("the value to remap the intensity curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapIntensityZero = 0f;

        /// the value to remap the range curve's 0 to
        [Tooltip("the value to remap the range curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapRangeOne = 10f;

        /// the value to remap the range curve's 0 to
        [Tooltip("the value to remap the range curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapRangeZero = 0f;

        /// the value to remap the shadow strength's curve's 1 to
        [Tooltip("the value to remap the shadow strength's curve's 1 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapShadowStrengthOne = 1f;

        /// the value to remap the shadow strength's curve's 0 to
        [Tooltip("the value to remap the shadow strength's curve's 0 to")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float RemapShadowStrengthZero = 0f;

        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public bool ResetShakerValuesAfterShake = true;

        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public bool ResetTargetValuesAfterShake = true;

        [Header("Shadow Strength")]
        /// the range to apply to the light over time
        [Tooltip("the range to apply to the light over time")]
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public AnimationCurve ShadowStrengthCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));

        /// whether or not that light should be turned off on start
        [Tooltip("whether or not that light should be turned off on start")]
        public bool StartsOff = true;

        /// whether or not to broadcast a range to only affect certain shakers
        [Tooltip("whether or not to broadcast a range to only affect certain shakers")]
        [MMFEnumCondition("Mode", (int)Modes.ShakerEvent)]
        public bool UseRange = false;

        /// the duration of this feedback is the duration of the light, or 0 if instant
        public override float FeedbackDuration
        {
            get => Mode == Modes.Instant ? 0f : ApplyTimeMultiplier(Duration);
            set => Duration = value;
        }

        public override bool HasChannel => true;

        /// <summary>
        ///     On init we turn the light off if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);

            if (BoundLight == null) return;

            _initialRange = BoundLight.range;
            _initialShadowStrength = BoundLight.shadowStrength;
            _initialIntensity = BoundLight.intensity;

            if (EventOriginTransform == null) EventOriginTransform = owner.transform;

            if (Active)
                if (StartsOff)
                    Turn(false);
        }

        /// <summary>
        ///     On Play we turn our light on and start an over time coroutine if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            var intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
            Turn(true);
            switch (Mode)
            {
                case Modes.Instant:
                    BoundLight.intensity = InstantIntensity * intensityMultiplier;
                    BoundLight.shadowStrength = InstantShadowStrength;
                    BoundLight.range = InstantRange;
                    if (ModifyColor) BoundLight.color = InstantColor;
                    break;
                case Modes.OverTime:
                    if (!AllowAdditivePlays && _coroutine != null) return;
                    _coroutine = Owner.StartCoroutine(LightSequence(intensityMultiplier));

                    break;
                case Modes.ShakerEvent:
                    MMLightShakeEvent.Trigger(FeedbackDuration, RelativeValues, ModifyColor, ColorOverTime,
                        IntensityCurve,
                        RemapIntensityZero, RemapIntensityOne, RangeCurve, RemapRangeZero * intensityMultiplier,
                        RemapRangeOne * intensityMultiplier,
                        ShadowStrengthCurve, RemapShadowStrengthZero, RemapShadowStrengthOne, feedbacksIntensity,
                        Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake,
                        UseRange, EventRange, EventOriginTransform.position);
                    break;
            }
        }

        /// <summary>
        ///     This coroutine will modify the intensity and color of the light over time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator LightSequence(float intensityMultiplier)
        {
            IsPlaying = true;
            var journey = NormalPlayDirection ? 0f : FeedbackDuration;
            while (journey >= 0 && journey <= FeedbackDuration && FeedbackDuration > 0)
            {
                var remappedTime = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);

                SetLightValues(remappedTime, intensityMultiplier);

                journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
                yield return null;
            }

            SetLightValues(FinalNormalizedTime, intensityMultiplier);
            if (StartsOff) Turn(false);
            IsPlaying = false;
            _coroutine = null;
            yield return null;
        }

        /// <summary>
        ///     Sets the various values on the light on a specified time (between 0 and 1)
        /// </summary>
        /// <param name="time"></param>
        protected virtual void SetLightValues(float time, float intensityMultiplier)
        {
            var intensity = MMFeedbacksHelpers.Remap(IntensityCurve.Evaluate(time), 0f, 1f, RemapIntensityZero,
                RemapIntensityOne);
            var range = MMFeedbacksHelpers.Remap(RangeCurve.Evaluate(time), 0f, 1f, RemapRangeZero, RemapRangeOne);
            var shadowStrength = MMFeedbacksHelpers.Remap(ShadowStrengthCurve.Evaluate(time), 0f, 1f,
                RemapShadowStrengthZero, RemapShadowStrengthOne);

            if (RelativeValues)
            {
                intensity += _initialIntensity;
                shadowStrength += _initialShadowStrength;
                range += _initialRange;
            }

            BoundLight.intensity = intensity * intensityMultiplier;
            BoundLight.range = range;
            BoundLight.shadowStrength = Mathf.Clamp01(shadowStrength);
            if (ModifyColor) BoundLight.color = ColorOverTime.Evaluate(time);
        }

        /// <summary>
        ///     Turns the light off on stop
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!FeedbackTypeAuthorized) return;

            base.CustomStopFeedback(position, feedbacksIntensity);
            IsPlaying = false;
            if (Active && _coroutine != null)
            {
                Owner.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (Active && DisableOnStop) Turn(false);
        }

        /// <summary>
        ///     Turns the light on or off
        /// </summary>
        /// <param name="status"></param>
        protected virtual void Turn(bool status)
        {
            BoundLight.gameObject.SetActive(status);
            BoundLight.enabled = status;
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.LightColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return BoundLight == null;
        }

        public override string RequiredTargetText => BoundLight != null ? BoundLight.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a BoundLight be set to be able to work properly. You can set one below.";
#endif
    }
}