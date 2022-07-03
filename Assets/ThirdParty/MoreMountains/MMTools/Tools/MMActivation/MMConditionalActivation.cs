﻿using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Add this component to a gameobject, and it'll let you enable target monos after all other targets have been
    ///     disabled
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/Activation/MMConditionalActivation")]
    public class MMConditionalActivation : MonoBehaviour
    {
        /// a list of monos to enable
        public MonoBehaviour[] EnableThese;

        /// a list of all the monos that have to have been disabled first
        public MonoBehaviour[] AfterTheseAreAllDisabled;

        protected bool _enabled;

        /// <summary>
        ///     On update, we check if we should disable
        /// </summary>
        protected virtual void Update()
        {
            if (_enabled) return;

            var allDisabled = true;
            foreach (var component in AfterTheseAreAllDisabled)
                if (component.isActiveAndEnabled)
                    allDisabled = false;
            if (allDisabled)
            {
                foreach (var component in EnableThese) component.enabled = true;
                _enabled = true;
            }
        }
    }
}