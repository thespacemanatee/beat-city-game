using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	/// <summary>
	///     This feedback will let you update a TMP text value over time, with a value going from A to B over time, on a curve
	/// </summary>
	[AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback will let you update a TMP text value over time, with a value going from A to B over time, on a curve")]
    [FeedbackPath("TextMesh Pro/TMP Count To")]
    public class MMF_TMPCountTo : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected float _lastRefreshAt;

        protected string _newText;
        protected float _startTime;

        [MMFInspectorGroup("Count Settings", true, 13)]
        /// the value from which to count from
        [Tooltip("the value from which to count from")]
        public float CountFrom = 0f;

        /// the curve on which to animate the count
        [Tooltip("the curve on which to animate the count")]
        public MMTweenType CountingCurve =
            new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));

        /// the value to count towards
        [Tooltip("the value to count towards")]
        public float CountTo = 10f;

        /// the duration of the count, in seconds
        [Tooltip("the duration of the count, in seconds")]
        public float Duration = 5f;

        /// whether or not value should be floored
        [Tooltip("whether or not value should be floored")]
        public bool FloorValues = true;

        /// the format with which to display the count
        [Tooltip("the format with which to display the count")]
        public string Format = "00.00";

        /// the minimum frequency (in seconds) at which to refresh the text field
        [Tooltip("the minimum frequency (in seconds) at which to refresh the text field")]
        public float MinRefreshFrequency = 0f;

        [MMFInspectorGroup("TextMeshPro Target Text", true, 12, true)]
        /// the target TMP_Text component we want to change the text on
        [Tooltip("the target TMP_Text component we want to change the text on")]
        public TMP_Text TargetTMPText;

        /// the duration of this feedback is the duration of the scale animation
        public override float FeedbackDuration
        {
            get => ApplyTimeMultiplier(Duration);
            set => Duration = value;
        }

        /// <summary>
        ///     On play we change the text of our target TMPText over time
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            if (TargetTMPText == null) return;

            Owner.StartCoroutine(CountCo());
        }

        /// <summary>
        ///     A coroutine used to animate the text
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CountCo()
        {
            _lastRefreshAt = -float.MaxValue;
            var currentValue = CountFrom;
            _startTime = FeedbackTime;

            while (FeedbackTime - _startTime <= Duration)
            {
                if (FeedbackTime - _lastRefreshAt >= MinRefreshFrequency)
                {
                    currentValue = ProcessCount();
                    UpdateText(currentValue);
                    _lastRefreshAt = FeedbackTime;
                }

                yield return null;
            }

            UpdateText(CountTo);
        }

        /// <summary>
        ///     Updates the text of the target TMPText component with the updated value
        /// </summary>
        /// <param name="currentValue"></param>
        protected virtual void UpdateText(float currentValue)
        {
            if (FloorValues)
                _newText = Mathf.Floor(currentValue).ToString(Format);
            else
                _newText = currentValue.ToString(Format);

            TargetTMPText.text = _newText;
        }

        /// <summary>
        ///     Computes the new value of the count for the current time
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        protected virtual float ProcessCount()
        {
            var currentTime = FeedbackTime - _startTime;
            var currentValue = MMTween.Tween(currentTime, 0f, Duration, CountFrom, CountTo, CountingCurve);
            return currentValue;
        }
#if UNITY_EDITOR
        public override Color FeedbackColor => MMFeedbacksInspectorColors.TMPColor;

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