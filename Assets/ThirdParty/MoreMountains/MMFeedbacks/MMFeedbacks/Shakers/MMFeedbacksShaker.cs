using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [RequireComponent(typeof(MMFeedbacks))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Feedbacks/MMFeedbacksShaker")]
    public class MMFeedbacksShaker : MMShaker
    {
        protected MMFeedbacks _mmFeedbacks;

        /// <summary>
        ///     When that shaker gets added, we initialize its shake duration
        /// </summary>
        protected virtual void Reset()
        {
            ShakeDuration = 0.01f;
        }

        /// <summary>
        ///     On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _mmFeedbacks = gameObject.GetComponent<MMFeedbacks>();
        }

        public virtual void OnMMFeedbacksShakeEvent(int channel = 0, bool useRange = false, float eventRange = 0f,
            Vector3 eventOriginPosition = default)
        {
            if (!CheckEventAllowed(channel, useRange, eventRange, eventOriginPosition) ||
                (!Interruptible && Shaking)) return;
            Play();
        }

        protected override void ShakeStarts()
        {
            _mmFeedbacks.PlayFeedbacks();
        }

        /// <summary>
        ///     Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMFeedbacksShakeEvent.Register(OnMMFeedbacksShakeEvent);
        }

        /// <summary>
        ///     Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMFeedbacksShakeEvent.Unregister(OnMMFeedbacksShakeEvent);
        }
    }

    public struct MMFeedbacksShakeEvent
    {
        public delegate void Delegate(int channel = 0, bool useRange = false, float eventRange = 0f,
            Vector3 eventOriginPosition = default);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(int channel = 0, bool useRange = false, float eventRange = 0f,
            Vector3 eventOriginPosition = default)
        {
            OnEvent?.Invoke(channel, useRange, eventRange, eventOriginPosition);
        }
    }
}