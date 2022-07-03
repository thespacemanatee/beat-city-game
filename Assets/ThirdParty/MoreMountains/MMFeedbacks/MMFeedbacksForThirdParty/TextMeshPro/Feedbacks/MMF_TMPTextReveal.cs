using System.Collections;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback will let you reveal words, lines, or characters in a target TMP, one at a time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you reveal words, lines, or characters in a target TMP, one at a time")]
    [FeedbackPath("TextMesh Pro/TMP Text Reveal")]
    public class MMF_TMPTextReveal : MMF_Feedback
    {
        /// whether to define duration by the time interval between two unit reveals, or by the total duration the reveal should take
        public enum DurationModes
        {
            Interval,
            TotalDuration
        }

        /// the possible ways to reveal the text
        public enum RevealModes
        {
            Character,
            Lines,
            Words
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected Coroutine _coroutine;

        protected float _delay;
        protected int _richTextLength;

        /// whether to define duration by the time interval between two unit reveals, or by the total duration the reveal should take
        [Tooltip(
            "whether to define duration by the time interval between two unit reveals, or by the total duration the reveal should take")]
        public DurationModes DurationMode = DurationModes.Interval;

        /// the interval (in seconds) between two reveals
        [Tooltip("the interval (in seconds) between two reveals")]
        [MMFEnumCondition("DurationMode", (int)DurationModes.Interval)]
        public float IntervalBetweenReveals = 0.05f;

        /// the new text to replace the old one with
        [Tooltip("the new text to replace the old one with")] [TextArea]
        public string NewText = "Hello World";

        [MMFInspectorGroup("Change Text", true, 13)]
        /// whether or not to replace the current TMP target's text on play
        [Tooltip("whether or not to replace the current TMP target's text on play")]
        public bool ReplaceText = false;

        /// the total duration of the text reveal, in seconds
        [Tooltip("the total duration of the text reveal, in seconds")]
        [MMFEnumCondition("DurationMode", (int)DurationModes.TotalDuration)]
        public float RevealDuration = 1f;

        [MMFInspectorGroup("Reveal", true, 14)]
        /// the selected way to reveal the text (character by character, word by word, or line by line)
        [Tooltip("the selected way to reveal the text (character by character, word by word, or line by line)")]
        public RevealModes RevealMode = RevealModes.Character;

        [MMFInspectorGroup("Target", true, 12, true)]
        /// the target TMP_Text component we want to change the text on
        [Tooltip("the target TMP_Text component we want to change the text on")]
        public TMP_Text TargetTMPText;

        /// the duration of this feedback
        public override float FeedbackDuration
        {
            get
            {
                if (DurationMode == DurationModes.TotalDuration) return RevealDuration;

                if (TargetTMPText == null || TargetTMPText.textInfo == null) return 0f;

                switch (RevealMode)
                {
                    case RevealModes.Character:
                        return RichTextLength(TargetTMPText.text) * IntervalBetweenReveals;
                    case RevealModes.Lines:
                        return TargetTMPText.textInfo.lineCount * IntervalBetweenReveals;
                    case RevealModes.Words:
                        return TargetTMPText.textInfo.wordCount * IntervalBetweenReveals;
                }

                return 0f;
            }
            set
            {
                if (DurationMode == DurationModes.TotalDuration)
                {
                    RevealDuration = value;
                }
                else
                {
                    if (TargetTMPText != null)
                        switch (RevealMode)
                        {
                            case RevealModes.Character:
                                IntervalBetweenReveals = value / RichTextLength(TargetTMPText.text);
                                break;
                            case RevealModes.Lines:
                                IntervalBetweenReveals = value / TargetTMPText.textInfo.lineCount;
                                break;
                            case RevealModes.Words:
                                IntervalBetweenReveals = value / TargetTMPText.textInfo.wordCount;
                                break;
                        }
                }
            }
        }

        /// <summary>
        ///     On play we change the text of our target TMPText
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            if (TargetTMPText == null) return;

            if (ReplaceText)
            {
                TargetTMPText.text = NewText;
                TargetTMPText.ForceMeshUpdate();
            }

            _richTextLength = RichTextLength(TargetTMPText.text);
            switch (RevealMode)
            {
                case RevealModes.Character:
                    _delay = DurationMode == DurationModes.Interval
                        ? IntervalBetweenReveals
                        : RevealDuration / _richTextLength;
                    TargetTMPText.maxVisibleCharacters = 0;
                    _coroutine = Owner.StartCoroutine(RevealCharacters());
                    break;
                case RevealModes.Lines:
                    _delay = DurationMode == DurationModes.Interval
                        ? IntervalBetweenReveals
                        : RevealDuration / TargetTMPText.textInfo.lineCount;
                    TargetTMPText.maxVisibleLines = 0;
                    _coroutine = Owner.StartCoroutine(RevealLines());
                    break;
                case RevealModes.Words:
                    _delay = DurationMode == DurationModes.Interval
                        ? IntervalBetweenReveals
                        : RevealDuration / TargetTMPText.textInfo.wordCount;
                    TargetTMPText.maxVisibleWords = 0;
                    _coroutine = Owner.StartCoroutine(RevealWords());
                    break;
            }
        }

        /// <summary>
        ///     Reveals characters one at a time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator RevealCharacters()
        {
            var startTime = Timing.TimescaleMode == TimescaleModes.Scaled ? Time.time : Time.unscaledTime;
            var totalCharacters = _richTextLength;
            var visibleCharacters = 0;
            var lastCharAt = 0f;

            IsPlaying = true;
            while (visibleCharacters <= totalCharacters)
            {
                var deltaTime = Timing.TimescaleMode == TimescaleModes.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;
                var time = Timing.TimescaleMode == TimescaleModes.Scaled ? Time.time : Time.unscaledTime;

                if (time - lastCharAt < IntervalBetweenReveals) yield return null;

                TargetTMPText.maxVisibleCharacters = visibleCharacters;
                visibleCharacters++;
                lastCharAt = time;

                // we adjust our delay

                var delay = 0f;

                if (DurationMode == DurationModes.Interval)
                {
                    _delay = Mathf.Max(IntervalBetweenReveals, deltaTime);
                    delay = _delay - deltaTime;
                }
                else
                {
                    var remainingCharacters = totalCharacters - visibleCharacters;
                    var elapsedTime = time - startTime;
                    if (remainingCharacters != 0) _delay = (RevealDuration - elapsedTime) / remainingCharacters;
                    delay = _delay - deltaTime;
                }

                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    yield return MMFeedbacksCoroutine.WaitFor(delay);
                else
                    yield return MMFeedbacksCoroutine.WaitForUnscaled(delay);
            }

            TargetTMPText.maxVisibleCharacters = _richTextLength;
            IsPlaying = false;
        }

        /// <summary>
        ///     Reveals lines one at a time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator RevealLines()
        {
            var totalLines = TargetTMPText.textInfo.lineCount;
            var visibleLines = 0;

            IsPlaying = true;
            while (visibleLines <= totalLines)
            {
                TargetTMPText.maxVisibleLines = visibleLines;
                visibleLines++;

                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    yield return MMFeedbacksCoroutine.WaitFor(_delay);
                else
                    yield return MMFeedbacksCoroutine.WaitForUnscaled(_delay);
            }

            IsPlaying = false;
        }

        /// <summary>
        ///     Reveals words one at a time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator RevealWords()
        {
            var totalWords = TargetTMPText.textInfo.wordCount;
            var visibleWords = 0;

            IsPlaying = true;
            while (visibleWords <= totalWords)
            {
                TargetTMPText.maxVisibleWords = visibleWords;
                visibleWords++;

                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    yield return MMFeedbacksCoroutine.WaitFor(_delay);
                else
                    yield return MMFeedbacksCoroutine.WaitForUnscaled(_delay);
            }

            IsPlaying = false;
        }

        /// <summary>
        ///     Stops the animation if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized) return;
            base.CustomStopFeedback(position, feedbacksIntensity);
            IsPlaying = false;
            if (_coroutine != null)
            {
                Owner.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        /// <summary>
        ///     Returns the length of a rich text, excluding its tags
        /// </summary>
        /// <param name="richText"></param>
        /// <returns></returns>
        protected int RichTextLength(string richText)
        {
            var richTextLength = 0;
            var insideTag = false;

            richText = richText.Replace("<br>", "-");

            foreach (var character in richText)
                if (character == '<')
                {
                    insideTag = true;
                }
                else if (character == '>')
                {
                    insideTag = false;
                }
                else if (!insideTag)
                {
                    richTextLength++;
                }

            return richTextLength;
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