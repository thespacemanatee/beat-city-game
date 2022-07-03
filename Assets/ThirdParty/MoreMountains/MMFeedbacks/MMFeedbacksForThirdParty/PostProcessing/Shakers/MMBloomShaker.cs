using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    ///     Add this class to a Camera with a bloom post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMBloomShaker")]
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMBloomShaker : MMShaker
    {
        /// whether or not to add to the initial value
        public bool RelativeValues = true;

        [Header("Intensity")]
        /// the curve used to animate the intensity value on
        [Tooltip("the curve used to animate the intensity value on")]
        public AnimationCurve ShakeIntensity = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        public float RemapIntensityZero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        public float RemapIntensityOne = 10f;

        [Header("Threshold")]
        /// the curve used to animate the threshold value on
        [Tooltip("the curve used to animate the threshold value on")]
        public AnimationCurve ShakeThreshold = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        public float RemapThresholdZero;

        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        public float RemapThresholdOne;

        protected Bloom _bloom;
        protected float _initialIntensity;
        protected float _initialThreshold;
        protected bool _originalRelativeIntensity;
        protected float _originalRemapIntensityOne;
        protected float _originalRemapIntensityZero;
        protected float _originalRemapThresholdOne;
        protected float _originalRemapThresholdZero;
        protected float _originalShakeDuration;
        protected AnimationCurve _originalShakeIntensity;
        protected AnimationCurve _originalShakeThreshold;

        protected PostProcessVolume _volume;

        /// <summary>
        ///     On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _volume = gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _bloom);
        }

        /// <summary>
        ///     Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            var newIntensity = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeValues,
                _initialIntensity);
            _bloom.intensity.Override(newIntensity);
            var newThreshold = ShakeFloat(ShakeThreshold, RemapThresholdZero, RemapThresholdOne, RelativeValues,
                _initialThreshold);
            _bloom.threshold.Override(newThreshold);
        }

        /// <summary>
        ///     Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialIntensity = _bloom.intensity;
            _initialThreshold = _bloom.threshold;
        }

        /// <summary>
        ///     When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeIntensity"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <param name="channel"></param>
        public virtual void OnBloomShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax,
            AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true,
            bool resetTargetValuesAfterShake = true, bool forwardDirection = true,
            TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            if (!CheckEventAllowed(channel) || (!Interruptible && Shaking)) return;

            if (stop)
            {
                Stop();
                return;
            }

            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeIntensity = ShakeIntensity;
                _originalRemapIntensityZero = RemapIntensityZero;
                _originalRemapIntensityOne = RemapIntensityOne;
                _originalRelativeIntensity = RelativeValues;
                _originalShakeThreshold = ShakeThreshold;
                _originalRemapThresholdZero = RemapThresholdZero;
                _originalRemapThresholdOne = RemapThresholdOne;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeIntensity = intensity;
            RemapIntensityZero = remapMin * feedbacksIntensity;
            RemapIntensityOne = remapMax * feedbacksIntensity;
            RelativeValues = relativeIntensity;
            ShakeThreshold = threshold;
            RemapThresholdZero = remapThresholdMin;
            RemapThresholdOne = remapThresholdMax;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        ///     Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _bloom.intensity.Override(_initialIntensity);
            _bloom.threshold.Override(_initialThreshold);
        }

        /// <summary>
        ///     Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeIntensity = _originalShakeIntensity;
            RemapIntensityZero = _originalRemapIntensityZero;
            RemapIntensityOne = _originalRemapIntensityOne;
            RelativeValues = _originalRelativeIntensity;
            ShakeThreshold = _originalShakeThreshold;
            RemapThresholdZero = _originalRemapThresholdZero;
            RemapThresholdOne = _originalRemapThresholdOne;
        }

        /// <summary>
        ///     Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMBloomShakeEvent.Register(OnBloomShakeEvent);
        }

        /// <summary>
        ///     Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMBloomShakeEvent.Unregister(OnBloomShakeEvent);
        }
    }

    /// <summary>
    ///     An event used to trigger vignette shakes
    /// </summary>
    public struct MMBloomShakeEvent
    {
        public delegate void Delegate(AnimationCurve intensity, float duration, float remapMin, float remapMax,
            AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true,
            bool resetTargetValuesAfterShake = true, bool forwardDirection = true,
            TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(AnimationCurve intensity, float duration, float remapMin, float remapMax,
            AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true,
            bool resetTargetValuesAfterShake = true, bool forwardDirection = true,
            TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            OnEvent?.Invoke(intensity, duration, remapMin, remapMax, threshold, remapThresholdMin, remapThresholdMax,
                relativeIntensity,
                feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection,
                timescaleMode, stop);
        }
    }
}