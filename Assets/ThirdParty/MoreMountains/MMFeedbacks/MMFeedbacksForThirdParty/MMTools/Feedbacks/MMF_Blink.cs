using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will trigger a MMBlink object, letting you blink something
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you trigger a blink on an MMBlink object.")]
    [FeedbackPath("Renderer/MMBlink")]
    public class MMF_Blink : MMF_Feedback
    {
        /// the possible modes for this feedback, that correspond to MMBlink's public methods
        public enum BlinkModes
        {
            Toggle,
            Start,
            Stop
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// the selected mode for this feedback
        [Tooltip("the selected mode for this feedback")]
        public BlinkModes BlinkMode = BlinkModes.Toggle;

        [MMFInspectorGroup("Blink", true, 61, true)]
        /// the target object to blink
        [Tooltip("the target object to blink")]
        public MMBlink TargetBlink;

        /// <summary>
        ///     On Custom play, we trigger our MMBlink object
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || TargetBlink == null) return;
            TargetBlink.TimescaleMode = Timing.TimescaleMode;
            switch (BlinkMode)
            {
                case BlinkModes.Toggle:
                    TargetBlink.ToggleBlinking();
                    break;
                case BlinkModes.Start:
                    TargetBlink.StartBlinking();
                    break;
                case BlinkModes.Stop:
                    TargetBlink.StopBlinking();
                    break;
            }
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get => MMFeedbacksInspectorColors.RendererColor;
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetBlink == null;
        }

        public override string RequiredTargetText => TargetBlink != null ? TargetBlink.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetBlink be set to be able to work properly. You can set one below.";
#endif
    }
}