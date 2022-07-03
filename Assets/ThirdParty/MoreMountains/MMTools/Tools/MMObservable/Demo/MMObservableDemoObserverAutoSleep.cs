using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A test class used to demonstrate the MMObservable pattern in the MMObservableDemo scene
    ///     This one disables itself on Awake, and passively listens for changes, even when disabled
    /// </summary>
    public class MMObservableDemoObserverAutoSleep : MonoBehaviour
    {
        public MMObservableDemoSubject TargetSubject;

        /// <summary>
        ///     On awake we start listening for changes
        /// </summary>
        protected virtual void Awake()
        {
            TargetSubject.PositionX.OnValueChanged += OnSpeedChange;
            enabled = false;
        }

        /// <summary>
        ///     On enable we do nothing
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        ///     On disable we do nothing
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        ///     On destroy we stop listening for changes
        /// </summary>
        protected virtual void OnDestroy()
        {
            TargetSubject.PositionX.OnValueChanged -= OnSpeedChange;
        }

        protected virtual void OnSpeedChange()
        {
            transform.position = transform.position.MMSetY(TargetSubject.PositionX.Value);
        }
    }
}