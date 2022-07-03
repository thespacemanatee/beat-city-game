﻿using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     A class used on the MMFeedback's demo ghost
    /// </summary>
    public class DemoGhost : MonoBehaviour
    {
        /// <summary>
        ///     Called via animation event, disables the object
        /// </summary>
        public virtual void OnAnimationEnd()
        {
            gameObject.SetActive(false);
        }
    }
}