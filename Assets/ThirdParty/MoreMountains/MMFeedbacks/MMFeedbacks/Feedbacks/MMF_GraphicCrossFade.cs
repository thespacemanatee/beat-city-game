using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you trigger cross fades on a target Graphic.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you trigger cross fades on a target Graphic.")]
    [FeedbackPath("UI/Graphic CrossFade")]
    public class MMF_GraphicCrossFade : MMF_Feedback
    {
        /// the possible modes for this feedback
        public enum Modes
        {
            Alpha,
            Color
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected Coroutine _coroutine;
        protected Color _initialColor;

        /// how long the Graphic should change over time
        [Tooltip("how long the Graphic should change over time")]
        public float Duration = 0.2f;

        /// whether the feedback should affect the Image instantly or over a period of time
        [Tooltip("whether the feedback should affect the Image instantly or over a period of time")]
        public Modes Mode = Modes.Alpha;

        /// the target alpha
        [Tooltip("the target alpha")] [MMFEnumCondition("Mode", (int)Modes.Alpha)]
        public float TargetAlpha = 0.2f;

        /// the target color
        [Tooltip("the target color")] [MMFEnumCondition("Mode", (int)Modes.Color)]
        public Color TargetColor = Color.red;

        [MMFInspectorGroup("Graphic Cross Fade", true, 54, true)]
        /// the Graphic to affect when playing the feedback
        [Tooltip("the Graphic to affect when playing the feedback")]
        public Graphic TargetGraphic;

        /// whether or not the crossfade should also tween the alpha channel
        [Tooltip("whether or not the crossfade should also tween the alpha channel")]
        [MMFEnumCondition("Mode", (int)Modes.Color)]
        public bool UseAlpha = true;

        /// the duration of this feedback is the duration of the Image, or 0 if instant
        public override float FeedbackDuration
        {
            get => ApplyTimeMultiplier(Duration);
            set => Duration = value;
        }

        public override bool HasChannel => true;

        /// <summary>
        ///     On Play we turn our Graphic on and start an over time coroutine if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || TargetGraphic == null) return;

            Turn(true);
            var ignoreTimeScale = Timing.TimescaleMode == TimescaleModes.Unscaled;
            switch (Mode)
            {
                case Modes.Alpha:
                    // the following lines fix a bug with CrossFadeAlpha
                    _initialColor = TargetGraphic.color;
                    _initialColor.a = 1;
                    TargetGraphic.color = _initialColor;
                    TargetGraphic.CrossFadeAlpha(0f, 0f, true);

                    TargetGraphic.CrossFadeAlpha(TargetAlpha, Duration, ignoreTimeScale);
                    break;
                case Modes.Color:
                    TargetGraphic.CrossFadeColor(TargetColor, Duration, ignoreTimeScale, UseAlpha);
                    break;
            }
        }

        /// <summary>
        ///     Turns the Graphic off on stop
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized) return;
            IsPlaying = false;
            base.CustomStopFeedback(position, feedbacksIntensity);
            Turn(false);
        }

        /// <summary>
        ///     Turns the Graphic on or off
        /// </summary>
        /// <param name="status"></param>
        protected virtual void Turn(bool status)
        {
            TargetGraphic.gameObject.SetActive(status);
            TargetGraphic.enabled = status;
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.UIColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetGraphic == null;
        }

        public override string RequiredTargetText => TargetGraphic != null ? TargetGraphic.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetGraphic be set to be able to work properly. You can set one below.";
#endif
    }
}