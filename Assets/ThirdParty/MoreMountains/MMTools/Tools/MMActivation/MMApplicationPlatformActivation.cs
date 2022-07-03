﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class used to store bindings
    /// </summary>
    [Serializable]
    public class PlatformBindings
    {
        public enum PlatformActions
        {
            DoNothing,
            Disable
        }

        public RuntimePlatform Platform = RuntimePlatform.WindowsPlayer;
        public PlatformActions PlatformAction = PlatformActions.DoNothing;
    }

    /// <summary>
    ///     Add this class to a gameobject, and it'll enable/disable it based on platform context, using Application.platform
    ///     to detect the platform
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/Activation/MMApplicationPlatformActivation")]
    public class MMApplicationPlatformActivation : MonoBehaviour
    {
        /// the possible times at which this script can run
        public enum ExecutionTimes
        {
            Awake,
            Start,
            OnEnable
        }

        [Header("Settings")]
        /// the selected execution time
        public ExecutionTimes ExecutionTime = ExecutionTimes.Awake;

        /// whether or not this should output a debug line in the console
        public bool DebugToTheConsole;

        [Header("Platforms")] public List<PlatformBindings> Platforms;

        /// <summary>
        ///     On Awake, processes the state if needed
        /// </summary>
        protected virtual void Awake()
        {
            if (ExecutionTime == ExecutionTimes.Awake) Process();
        }

        /// <summary>
        ///     On Start, processes the state if needed
        /// </summary>
        protected virtual void Start()
        {
            if (ExecutionTime == ExecutionTimes.Start) Process();
        }

        /// <summary>
        ///     On Enable, processes the state if needed
        /// </summary>
        protected virtual void OnEnable()
        {
            if (ExecutionTime == ExecutionTimes.OnEnable) Process();
        }

        /// <summary>
        ///     Enables or disables the object based on current platform
        /// </summary>
        protected virtual void Process()
        {
            foreach (var platform in Platforms)
                if (platform.Platform == Application.platform)
                    DisableIfNeeded(platform.PlatformAction, platform.Platform.ToString());

            if (Application.platform == RuntimePlatform.Android)
            {
            }
        }

        /// <summary>
        ///     Disables the object if needed, and outputs a debug log if requested
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="platformName"></param>
        protected virtual void DisableIfNeeded(PlatformBindings.PlatformActions platform, string platformName)
        {
            if (gameObject.activeInHierarchy && platform == PlatformBindings.PlatformActions.Disable)
            {
                gameObject.SetActive(false);
                if (DebugToTheConsole)
                    Debug.LogFormat(gameObject.name + " got disabled via MMPlatformActivation, platform : " +
                                    platformName + ".");
            }
        }
    }
}