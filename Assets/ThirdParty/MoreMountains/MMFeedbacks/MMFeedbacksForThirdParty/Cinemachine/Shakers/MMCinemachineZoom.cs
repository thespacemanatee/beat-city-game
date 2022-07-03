﻿using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    ///     This class will allow you to trigger zooms on your cinemachine camera by sending MMCameraZoomEvents from any other
    ///     class
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachineZoom")]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class MMCinemachineZoom : MonoBehaviour
    {
        public int Channel;

        [Header("Transition Speed")]
        /// the animation curve to apply to the zoom transition
        [Tooltip("the animation curve to apply to the zoom transition")]
        public AnimationCurve ZoomCurve = new(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        [Header("Test Zoom")]
        /// the mode to apply the zoom in when using the test button in the inspector
        [Tooltip("the mode to apply the zoom in when using the test button in the inspector")]
        public MMCameraZoomModes TestMode;

        /// the target field of view to apply the zoom in when using the test button in the inspector
        [Tooltip("the target field of view to apply the zoom in when using the test button in the inspector")]
        public float TestFieldOfView = 30f;

        /// the transition duration to apply the zoom in when using the test button in the inspector
        [Tooltip("the transition duration to apply the zoom in when using the test button in the inspector")]
        public float TestTransitionDuration = 0.1f;

        /// the duration to apply the zoom in when using the test button in the inspector
        [Tooltip("the duration to apply the zoom in when using the test button in the inspector")]
        public float TestDuration = 0.05f;

        [MMFInspectorButton("TestZoom")]
        /// an inspector button to test the zoom in play mode
        public bool TestZoomButton;

        protected float _delta;
        protected bool _destinationReached;
        protected int _direction = 1;
        protected float _duration;
        protected float _initialFieldOfView;
        protected MMCameraZoomModes _mode;
        protected float _reachedDestinationTimestamp;
        protected float _startFieldOfView;
        protected float _targetFieldOfView;
        protected float _transitionDuration;

        protected CinemachineVirtualCamera _virtualCamera;
        protected bool _zooming;

        public TimescaleModes TimescaleMode { get; set; }

        /// <summary>
        ///     On Awake we grab our virtual camera
        /// </summary>
        protected virtual void Awake()
        {
            _virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
            _initialFieldOfView = _virtualCamera.m_Lens.FieldOfView;
        }

        /// <summary>
        ///     On Update if we're zooming we modify our field of view accordingly
        /// </summary>
        protected virtual void Update()
        {
            if (!_zooming) return;

            if (_virtualCamera.m_Lens.FieldOfView != _targetFieldOfView)
            {
                _delta += GetDeltaTime() / _transitionDuration;
                _virtualCamera.m_Lens.FieldOfView =
                    Mathf.LerpUnclamped(_startFieldOfView, _targetFieldOfView, ZoomCurve.Evaluate(_delta));
            }
            else
            {
                if (!_destinationReached)
                {
                    _reachedDestinationTimestamp = GetTime();
                    _destinationReached = true;
                }

                if (_mode == MMCameraZoomModes.For && _direction == 1)
                {
                    if (GetTime() - _reachedDestinationTimestamp > _duration)
                    {
                        _direction = -1;
                        _startFieldOfView = _targetFieldOfView;
                        _targetFieldOfView = _initialFieldOfView;
                        _delta = 0f;
                    }
                }
                else
                {
                    _zooming = false;
                }
            }
        }

        /// <summary>
        ///     Starts listening for MMCameraZoomEvents
        /// </summary>
        protected virtual void OnEnable()
        {
            MMCameraZoomEvent.Register(OnCameraZoomEvent);
        }

        /// <summary>
        ///     Stops listening for MMCameraZoomEvents
        /// </summary>
        protected virtual void OnDisable()
        {
            MMCameraZoomEvent.Unregister(OnCameraZoomEvent);
        }

        public virtual float GetTime()
        {
            return TimescaleMode == TimescaleModes.Scaled ? Time.time : Time.unscaledTime;
        }

        public virtual float GetDeltaTime()
        {
            return TimescaleMode == TimescaleModes.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;
        }

        /// <summary>
        ///     A method that triggers the zoom, ideally only to be called via an event, but public for convenience
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="newFieldOfView"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="duration"></param>
        public virtual void Zoom(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration,
            bool useUnscaledTime, bool relative = false)
        {
            if (_zooming) return;

            _zooming = true;
            _delta = 0f;
            _mode = mode;

            TimescaleMode = useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled;
            _startFieldOfView = _virtualCamera.m_Lens.FieldOfView;
            _transitionDuration = transitionDuration;
            _duration = duration;
            _transitionDuration = transitionDuration;
            _direction = 1;
            _destinationReached = false;

            switch (mode)
            {
                case MMCameraZoomModes.For:
                    _targetFieldOfView = newFieldOfView;
                    break;

                case MMCameraZoomModes.Set:
                    _targetFieldOfView = newFieldOfView;
                    break;

                case MMCameraZoomModes.Reset:
                    _targetFieldOfView = _initialFieldOfView;
                    break;
            }

            if (relative) _targetFieldOfView += _initialFieldOfView;
        }

        /// <summary>
        ///     The method used by the test button to trigger a test zoom
        /// </summary>
        protected virtual void TestZoom()
        {
            Zoom(TestMode, TestFieldOfView, TestTransitionDuration, TestDuration, false);
        }

        /// <summary>
        ///     When we get an MMCameraZoomEvent we call our zoom method
        /// </summary>
        /// <param name="zoomEvent"></param>
        public virtual void OnCameraZoomEvent(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration,
            float duration, int channel, bool useUnscaledTime, bool stop = false, bool relative = false)
        {
            if (channel != Channel && channel != -1 && Channel != -1) return;
            if (stop)
            {
                _zooming = false;
                return;
            }

            Zoom(mode, newFieldOfView, transitionDuration, duration, useUnscaledTime, relative);
        }
    }
}