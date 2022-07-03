﻿using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you fade all the sounds on a specific track at once. You will need a MMSoundManager in your
    ///     scene for this to work.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/MMSoundManager Track Fade")]
    [FeedbackHelp(
        "This feedback will let you fade all the sounds on a specific track at once. You will need a MMSoundManager in your scene for this to work.")]
    public class MMF_MMSoundManagerTrackFade : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// the duration of the fade, in seconds
        [Tooltip("the duration of the fade, in seconds")]
        public float FadeDuration = 1f;

        /// the tween to operate the fade on
        [Tooltip("the tween to operate the fade on")]
        public MMTweenType FadeTween = new(MMTween.MMTweenCurve.EaseInOutQuartic);

        /// the volume to reach at the end of the fade
        [Tooltip("the volume to reach at the end of the fade")]
        [Range(MMSoundManagerSettings._minimalVolume, MMSoundManagerSettings._maxVolume)]
        public float FinalVolume = MMSoundManagerSettings._minimalVolume;

        [MMFInspectorGroup("MMSoundManager Track Fade", true, 30)]
        /// the track to fade the volume on
        [Tooltip("the track to fade the volume on")]
        public MMSoundManager.MMSoundManagerTracks Track;

        /// the duration of this feedback is the duration of the fade
        public override float FeedbackDuration => FadeDuration;

        /// <summary>
        ///     On Play, triggers a fade event, meant to be caught by the MMSoundManager
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            MMSoundManagerTrackFadeEvent.Trigger(Track, FadeDuration, FinalVolume, FadeTween);
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.SoundsColor; }
        }

        public override string RequiredTargetText => Track.ToString();
#endif
    }
}