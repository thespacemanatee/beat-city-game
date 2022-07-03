using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     This feedback lets you control the font size of a target Text over time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you control the font size of a target Text over time.")]
    [FeedbackPath("UI/Text Font Size")]
    public class MMF_TextFontSize : MMF_FeedbackBase
    {
        [MMFInspectorGroup("Font Size", true, 59)]
        /// the curve to tween on
        [Tooltip("the curve to tween on")]
        [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public MMTweenType FontSizeCurve =
            new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));

        /// the value to move to in instant mode
        [Tooltip("the value to move to in instant mode")] [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.Instant)]
        public float InstantFontSize;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")] [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public float RemapOne = 1f;

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")] [MMFEnumCondition("Mode", (int)MMFeedbackBase.Modes.OverTime)]
        public float RemapZero = 0f;

        [MMFInspectorGroup("Target", true, 58, true)]
        /// the TMP_Text component to control
        [Tooltip("the TMP_Text component to control")]
        public Text TargetText;

        protected override void FillTargets()
        {
            if (TargetText == null) return;

            var target = new MMF_FeedbackBaseTarget();
            var receiver = new MMPropertyReceiver();
            receiver.TargetObject = TargetText.gameObject;
            receiver.TargetComponent = TargetText;
            receiver.TargetPropertyName = "fontSize";
            receiver.RelativeValue = RelativeValues;
            target.Target = receiver;
            target.LevelCurve = FontSizeCurve;
            target.RemapLevelZero = RemapZero;
            target.RemapLevelOne = RemapOne;
            target.InstantLevel = InstantFontSize;

            _targets.Add(target);
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.TMPColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return TargetText == null;
        }

        public override string RequiredTargetText => TargetText != null ? TargetText.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a TargetText be set to be able to work properly. You can set one below.";
#endif
    }
}