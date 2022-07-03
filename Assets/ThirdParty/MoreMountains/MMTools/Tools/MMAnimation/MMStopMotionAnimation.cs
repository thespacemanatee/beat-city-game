using UnityEngine;

namespace MoreMountains.Tools
{
    [AddComponentMenu("More Mountains/Tools/Animation/MMStopMotionAnimation")]
    public class MMStopMotionAnimation : MonoBehaviour
    {
        public enum FramerateModes
        {
            Manual,
            Automatic
        }

        [Header("General Settings")] public bool StopMotionEnabled = true;

        public int AnimationLayerID;

        [Header("Framerate")] public FramerateModes FramerateMode = FramerateModes.Automatic;

        [MMEnumCondition("FramerateMode", (int)FramerateModes.Automatic)]
        public float FramesPerSecond = 4f;

        [MMEnumCondition("FramerateMode", (int)FramerateModes.Automatic)]
        public float PollFrequency = 1f;

        [MMEnumCondition("FramerateMode", (int)FramerateModes.Manual)]
        public float ManualTimeBetweenFrames = 0.125f;

        [MMEnumCondition("FramerateMode", (int)FramerateModes.Manual)]
        public float ManualAnimatorSpeed = 2;

        public float timet;
        protected Animator _animator;
        protected AnimationClip _currentClip;

        protected float _currentClipFPS;
        protected float _currentClipLength;
        protected float _lastPollAt = -10f;

        protected virtual void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            StopMotion();

            if (Time.time - _lastPollAt > PollFrequency) Poll();
        }

        protected virtual void StopMotion()
        {
            if (!StopMotionEnabled) return;

            var timeBetweenFrames = 0f;
            var animatorSpeed = 0f;

            switch (FramerateMode)
            {
                case FramerateModes.Manual:
                    timeBetweenFrames = ManualTimeBetweenFrames;
                    animatorSpeed = ManualAnimatorSpeed;
                    break;
                case FramerateModes.Automatic:
                    timeBetweenFrames = 1 / FramesPerSecond;
                    animatorSpeed = 1 / (FramesPerSecond - 1) * 2f * _currentClipFPS;
                    break;
            }

            timet += Time.deltaTime;
            if (timet > timeBetweenFrames)
            {
                timet -= timeBetweenFrames;
                _animator.speed = animatorSpeed;
            }
            else
            {
                _animator.speed = 0;
            }
        }

        protected virtual void Poll()
        {
            _currentClip = _animator.GetCurrentAnimatorClipInfo(AnimationLayerID)[0].clip;
            _currentClipLength = _currentClip.length;
            _currentClipFPS = _currentClip.frameRate;
            _lastPollAt = Time.time;
        }
    }
}