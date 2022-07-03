﻿using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you trigger save, load, and reset on MMSoundManager settings. You will need a MMSoundManager
    ///     in your scene for this to work.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/MMSoundManager Save and Load")]
    [FeedbackHelp(
        "This feedback will let you trigger save, load, and reset on MMSoundManager settings. You will need a MMSoundManager in your scene for this to work.")]
    public class MMFeedbackMMSoundManagerSaveLoad : MMFeedback
    {
        /// the possible modes you can use to interact with save settings
        public enum Modes
        {
            Save,
            Load,
            Reset
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        [Header("MMSoundManager Save and Load")]
        /// the selected mode to interact with save settings on the MMSoundManager
        [Tooltip("the selected mode to interact with save settings on the MMSoundManager")]
        public Modes Mode = Modes.Save;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.SoundsColor; }
        }
#endif

        /// <summary>
        ///     On Play, saves, loads or resets settings
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            switch (Mode)
            {
                case Modes.Save:
                    MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.SaveSettings);
                    break;
                case Modes.Load:
                    MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.LoadSettings);
                    break;
                case Modes.Reset:
                    MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.ResetSettings);
                    break;
            }
        }
    }
}