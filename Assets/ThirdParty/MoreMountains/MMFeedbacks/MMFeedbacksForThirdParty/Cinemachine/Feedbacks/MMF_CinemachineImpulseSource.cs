using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("Camera/Cinemachine Impulse Source")]
    [FeedbackHelp(
        "This feedback lets you generate an impulse on a Cinemachine Impulse source. You'll need a Cinemachine Impulse Listener on your camera for this to work.")]
    public class MMF_CinemachineImpulseSource : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// whether or not to clear impulses (stopping camera shakes) when the Stop method is called on that feedback
        [Tooltip(
            "whether or not to clear impulses (stopping camera shakes) when the Stop method is called on that feedback")]
        public bool ClearImpulseOnStop = false;

        [MMFInspectorGroup("Cinemachine Impulse Source", true, 28)]
        /// the impulse definition to broadcast
        [Tooltip("the impulse definition to broadcast")]
        public CinemachineImpulseSource ImpulseSource;

        /// the velocity to apply to the impulse shake
        [Tooltip("the velocity to apply to the impulse shake")]
        public Vector3 Velocity = new(1f, 1f, 1f);

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            if (ImpulseSource != null) ImpulseSource.GenerateImpulse(Velocity);
        }

        /// <summary>
        ///     Stops the animation if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized || !ClearImpulseOnStop) return;
            base.CustomStopFeedback(position, feedbacksIntensity);

            CinemachineImpulseManager.Instance.Clear();
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.CameraColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return ImpulseSource == null;
        }

        public override string RequiredTargetText => ImpulseSource != null ? ImpulseSource.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that an ImpulseSource be set to be able to work properly. You can set one below.";
#endif
    }
}