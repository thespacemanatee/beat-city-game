using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     Turns an object active or inactive at the various stages of the feedback
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback allows you to change the state of a behaviour on a target gameobject from active to inactive (or the opposite), on init, play, stop or reset. " +
        "For each of these you can specify if you want to force a state (enabled or disabled), or toggle it (enabled becomes disabled, disabled becomes enabled).")]
    [FeedbackPath("GameObject/Enable Behaviour")]
    public class MMF_Enable : MMF_Feedback
    {
        /// the possible effects the feedback can have on the target object's status
        public enum PossibleStates
        {
            Enabled,
            Disabled,
            Toggle
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// whether or not we should alter the state of the target object on init
        [Tooltip("whether or not we should alter the state of the target object on init")]
        public bool SetStateOnInit = false;

        /// whether or not we should alter the state of the target object on play
        [Tooltip("whether or not we should alter the state of the target object on play")]
        public bool SetStateOnPlay = false;

        /// whether or not we should alter the state of the target object on reset
        [Tooltip("whether or not we should alter the state of the target object on reset")]
        public bool SetStateOnReset = false;

        /// whether or not we should alter the state of the target object on stop
        [Tooltip("whether or not we should alter the state of the target object on stop")]
        public bool SetStateOnStop = false;

        /// how to change the state on init
        [MMFCondition("SetStateOnInit", true)] [Tooltip("how to change the state on init")]
        public PossibleStates StateOnInit = PossibleStates.Disabled;

        /// how to change the state on play
        [MMFCondition("SetStateOnPlay", true)] [Tooltip("how to change the state on play")]
        public PossibleStates StateOnPlay = PossibleStates.Disabled;

        /// how to change the state on reset
        [Tooltip("how to change the state on reset")] [MMFCondition("SetStateOnReset", true)]
        public PossibleStates StateOnReset = PossibleStates.Disabled;

        /// how to change the state on stop
        [Tooltip("how to change the state on stop")] [MMFCondition("SetStateOnStop", true)]
        public PossibleStates StateOnStop = PossibleStates.Disabled;

        [MMFInspectorGroup("Enable Target Monobehaviour", true, 86, true)]
        /// the gameobject we want to change the active state of
        [Tooltip("the gameobject we want to change the active state of")]
        public Behaviour TargetBehaviour;

        /// <summary>
        ///     On init we change the state of our Behaviour if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            if (Active && TargetBehaviour != null)
                if (SetStateOnInit)
                    SetStatus(StateOnInit);
        }

        /// <summary>
        ///     On Play we change the state of our Behaviour if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || TargetBehaviour == null) return;
            if (SetStateOnPlay) SetStatus(StateOnPlay);
        }

        /// <summary>
        ///     On Stop we change the state of our Behaviour if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized || TargetBehaviour == null) return;
            base.CustomStopFeedback(position, feedbacksIntensity);

            if (SetStateOnStop) SetStatus(StateOnStop);
        }

        /// <summary>
        ///     On Reset we change the state of our Behaviour if needed
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();

            if (InCooldown) return;

            if (!Active || !FeedbackTypeAuthorized || TargetBehaviour == null) return;

            if (SetStateOnReset) SetStatus(StateOnReset);
        }

        /// <summary>
        ///     Changes the status of the Behaviour
        /// </summary>
        /// <param name="state"></param>
        protected virtual void SetStatus(PossibleStates state)
        {
            switch (state)
            {
                case PossibleStates.Enabled:
                    TargetBehaviour.enabled = NormalPlayDirection ? true : false;
                    break;
                case PossibleStates.Disabled:
                    TargetBehaviour.enabled = NormalPlayDirection ? false : true;
                    break;
                case PossibleStates.Toggle:
                    TargetBehaviour.enabled = !TargetBehaviour.enabled;
                    break;
            }
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.GameObjectColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetBehaviour == null;
        }

        public override string RequiredTargetText => TargetBehaviour != null ? TargetBehaviour.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetBehaviour be set to be able to work properly. You can set one below.";
#endif
    }
}