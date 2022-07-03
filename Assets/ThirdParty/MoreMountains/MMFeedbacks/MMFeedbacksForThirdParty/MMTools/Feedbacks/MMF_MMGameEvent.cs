﻿using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will trigger a MMGameEvent of the specified name when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will trigger a MMGameEvent of the specified name when played")]
    [FeedbackPath("Events/MMGameEvent")]
    public class MMF_MMGameEvent : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        [MMFInspectorGroup("MMGameEvent", true, 57, true)]
        public string MMGameEventName;

        /// <summary>
        ///     On Play we change the values of our fog
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;
            MMGameEvent.Trigger(MMGameEventName);
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.EventsColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return MMGameEventName == "";
        }

        public override string RequiredTargetText => MMGameEventName;
        public override string RequiresSetupText => "This feedback requires that you specify a MMGameEventName below.";
#endif
    }
}