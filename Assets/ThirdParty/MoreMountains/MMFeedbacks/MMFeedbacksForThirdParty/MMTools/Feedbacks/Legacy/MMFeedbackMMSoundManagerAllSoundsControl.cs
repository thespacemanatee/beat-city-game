﻿using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     A feedback used to control all sounds playing on the MMSoundManager at once. It'll let you pause, play, stop and
    ///     free (stop and returns the audiosource to the pool) sounds.  You will need a MMSoundManager in your scene for this
    ///     to work.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/MMSoundManager All Sounds Control")]
    [FeedbackHelp(
        "A feedback used to control all sounds playing on the MMSoundManager at once. It'll let you pause, play, stop and free (stop and returns the audiosource to the pool) sounds. You will need a MMSoundManager in your scene for this to work.")]
    public class MMFeedbackMMSoundManagerAllSoundsControl : MMFeedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        [Header("MMSoundManager All Sounds Control")]
        /// The selected control mode. 
        [Tooltip("The selected control mode")]
        public MMSoundManagerAllSoundsControlEventTypes ControlMode = MMSoundManagerAllSoundsControlEventTypes.Pause;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.SoundsColor; }
        }
#endif

        /// <summary>
        ///     On Play, we call the specified event, to be caught by the MMSoundManager
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            switch (ControlMode)
            {
                case MMSoundManagerAllSoundsControlEventTypes.Pause:
                    MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.Pause);
                    break;
                case MMSoundManagerAllSoundsControlEventTypes.Play:
                    MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.Play);
                    break;
                case MMSoundManagerAllSoundsControlEventTypes.Stop:
                    MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.Stop);
                    break;
                case MMSoundManagerAllSoundsControlEventTypes.Free:
                    MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.Free);
                    break;
                case MMSoundManagerAllSoundsControlEventTypes.FreeAllButPersistent:
                    MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes
                        .FreeAllButPersistent);
                    break;
                case MMSoundManagerAllSoundsControlEventTypes.FreeAllLooping:
                    MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes
                        .FreeAllLooping);
                    break;
            }
        }
    }
}