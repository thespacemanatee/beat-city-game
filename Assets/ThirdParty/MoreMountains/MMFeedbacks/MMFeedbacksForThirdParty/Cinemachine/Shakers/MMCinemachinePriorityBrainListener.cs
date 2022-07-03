﻿using System.Collections;
using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    ///     Add this to a Cinemachine brain and it'll be able to accept custom blend transitions (used with
    ///     MMFeedbackCinemachineTransition)
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachinePriorityBrainListener")]
    [RequireComponent(typeof(CinemachineBrain))]
    public class MMCinemachinePriorityBrainListener : MonoBehaviour
    {
        [HideInInspector] public TimescaleModes TimescaleMode = TimescaleModes.Scaled;

        protected CinemachineBrain _brain;
        protected Coroutine _coroutine;
        protected CinemachineBlendDefinition _initialDefinition;

        /// <summary>
        ///     On Awake we grab our brain
        /// </summary>
        protected virtual void Awake()
        {
            _brain = gameObject.GetComponent<CinemachineBrain>();
        }

        /// <summary>
        ///     On enable we start listening for events
        /// </summary>
        protected virtual void OnEnable()
        {
            _coroutine = null;
            MMCinemachinePriorityEvent.Register(OnMMCinemachinePriorityEvent);
        }

        /// <summary>
        ///     Stops listening for events
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = null;
            MMCinemachinePriorityEvent.Unregister(OnMMCinemachinePriorityEvent);
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
        ///     When getting an event we change our default transition if needed
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="forceMaxPriority"></param>
        /// <param name="newPriority"></param>
        /// <param name="forceTransition"></param>
        /// <param name="blendDefinition"></param>
        /// <param name="resetValuesAfterTransition"></param>
        public virtual void OnMMCinemachinePriorityEvent(int channel, bool forceMaxPriority, int newPriority,
            bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition,
            TimescaleModes timescaleMode)
        {
            if (forceTransition)
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
                else
                    _initialDefinition = _brain.m_DefaultBlend;
                _brain.m_DefaultBlend = blendDefinition;
                TimescaleMode = timescaleMode;
                _coroutine = StartCoroutine(ResetBlendDefinition(blendDefinition.m_Time));
            }
        }

        /// <summary>
        ///     a coroutine used to reset the default transition to its initial value
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        protected virtual IEnumerator ResetBlendDefinition(float delay)
        {
            for (float timer = 0; timer < delay; timer += GetDeltaTime()) yield return null;
            _brain.m_DefaultBlend = _initialDefinition;
            _coroutine = null;
        }
    }
}