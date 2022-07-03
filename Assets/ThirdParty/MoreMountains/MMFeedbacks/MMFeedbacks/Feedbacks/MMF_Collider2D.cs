﻿using System;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you enable/disable/toggle a target collider 2D, or change its trigger status
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback will let you enable/disable/toggle a target collider 2D, or change its trigger status")]
    [FeedbackPath("GameObject/Collider2D")]
    public class MMF_Collider2D : MMF_Feedback
    {
        /// the possible effects the feedback can have on the target collider's status
        public enum Modes
        {
            Enable,
            Disable,
            ToggleActive,
            Trigger,
            NonTrigger,
            ToggleTrigger
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// the effect the feedback will have on the target collider's status
        public Modes Mode = Modes.Disable;

        [MMFInspectorGroup("Collider 2D", true, 12, true)]
        /// the collider to act upon
        [Tooltip("the collider to act upon")]
        public Collider2D TargetCollider2D;

        /// <summary>
        ///     On Play we change the state of our collider if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || TargetCollider2D == null) return;
            ApplyChanges(Mode);
        }

        /// <summary>
        ///     Changes the state of the collider
        /// </summary>
        /// <param name="state"></param>
        protected virtual void ApplyChanges(Modes mode)
        {
            switch (mode)
            {
                case Modes.Enable:
                    TargetCollider2D.enabled = true;
                    break;
                case Modes.Disable:
                    TargetCollider2D.enabled = false;
                    break;
                case Modes.ToggleActive:
                    TargetCollider2D.enabled = !TargetCollider2D.enabled;
                    break;
                case Modes.Trigger:
                    TargetCollider2D.isTrigger = true;
                    break;
                case Modes.NonTrigger:
                    TargetCollider2D.isTrigger = false;
                    break;
                case Modes.ToggleTrigger:
                    TargetCollider2D.isTrigger = !TargetCollider2D.isTrigger;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
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
            return TargetCollider2D == null;
        }

        public override string RequiredTargetText => TargetCollider2D != null ? TargetCollider2D.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetCollider2D be set to be able to work properly. You can set one below.";
#endif
    }
}