﻿using System.Collections;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback lets you control the color of a target TMP over time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you control the color of a target TMP over time.")]
    [FeedbackPath("TextMesh Pro/TMP Color")]
    public class MMF_TMPColor : MMF_Feedback
    {
        public enum ColorModes
        {
            Instant,
            Gradient,
            Interpolate
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected Coroutine _coroutine;

        protected Color _initialColor;

        /// if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over
        [Tooltip(
            "if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
        public bool AllowAdditivePlays = false;

        /// the curve to use when interpolating towards the destination color
        [Tooltip("the curve to use when interpolating towards the destination color")]
        [MMFEnumCondition("ColorMode", (int)ColorModes.Interpolate)]
        public AnimationCurve ColorCurve = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the gradient to use to animate the color over time
        [Tooltip("the gradient to use to animate the color over time")]
        [MMFEnumCondition("ColorMode", (int)ColorModes.Gradient)]
        [GradientUsage(true)]
        public Gradient ColorGradient;

        [MMFInspectorGroup("Color", true, 16)]
        /// the selected color mode :
        /// None : nothing will happen,
        /// gradient : evaluates the color over time on that gradient, from left to right,
        /// interpolate : lerps from the current color to the destination one 
        [Tooltip("the selected color mode :" +
                 "None : nothing will happen," +
                 "gradient : evaluates the color over time on that gradient, from left to right," +
                 "interpolate : lerps from the current color to the destination one ")]
        public ColorModes ColorMode = ColorModes.Interpolate;

        /// the destination color when in interpolate mode
        [Tooltip("the destination color when in interpolate mode")]
        [MMFEnumCondition("ColorMode", (int)ColorModes.Interpolate)]
        public Color DestinationColor = Color.yellow;

        /// how long the color of the text should change over time
        [Tooltip("how long the color of the text should change over time")]
        [MMFEnumCondition("ColorMode", (int)ColorModes.Interpolate, (int)ColorModes.Gradient)]
        public float Duration = 0.2f;

        /// the color to apply
        [Tooltip("the color to apply")] [MMFEnumCondition("ColorMode", (int)ColorModes.Instant)]
        public Color InstantColor = Color.yellow;

        [MMFInspectorGroup("Target", true, 12, true)]
        /// the TMP_Text component to control
        [Tooltip(" TMP_Text component to control")]
        public TMP_Text TargetTMPText;

        /// the duration of this feedback is the duration of the color transition, or 0 if instant
        public override float FeedbackDuration
        {
            get => ColorMode == ColorModes.Instant ? 0f : ApplyTimeMultiplier(Duration);
            set => Duration = value;
        }

        /// <summary>
        ///     On init we store our initial color
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);

            if (TargetTMPText == null) return;

            _initialColor = TargetTMPText.color;
        }

        /// <summary>
        ///     On Play we change our text's color
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            if (TargetTMPText == null) return;

            switch (ColorMode)
            {
                case ColorModes.Instant:
                    TargetTMPText.color = InstantColor;
                    break;
                case ColorModes.Gradient:
                    if (!AllowAdditivePlays && _coroutine != null) return;
                    _coroutine = Owner.StartCoroutine(ChangeColor());
                    break;
                case ColorModes.Interpolate:
                    if (!AllowAdditivePlays && _coroutine != null) return;
                    _coroutine = Owner.StartCoroutine(ChangeColor());
                    break;
            }
        }

        /// <summary>
        ///     Changes the color of the text over time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator ChangeColor()
        {
            var journey = NormalPlayDirection ? 0f : FeedbackDuration;
            IsPlaying = true;
            while (journey >= 0 && journey <= FeedbackDuration && FeedbackDuration > 0)
            {
                var remappedTime = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);

                SetColor(remappedTime);

                journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
                yield return null;
            }

            SetColor(FinalNormalizedTime);
            _coroutine = null;
            IsPlaying = false;
        }

        /// <summary>
        ///     Stops the animation if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized) return;
            base.CustomStopFeedback(position, feedbacksIntensity);
            IsPlaying = false;
            if (_coroutine != null)
            {
                Owner.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        /// <summary>
        ///     Applies the color change
        /// </summary>
        /// <param name="time"></param>
        protected virtual void SetColor(float time)
        {
            if (ColorMode == ColorModes.Gradient)
            {
                TargetTMPText.color = ColorGradient.Evaluate(time);
            }
            else if (ColorMode == ColorModes.Interpolate)
            {
                var factor = ColorCurve.Evaluate(time);
                TargetTMPText.color = Color.LerpUnclamped(_initialColor, DestinationColor, factor);
            }
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.TMPColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetTMPText == null;
        }

        public override string RequiredTargetText => TargetTMPText != null ? TargetTMPText.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetTMPText be set to be able to work properly. You can set one below.";
#endif
    }
}