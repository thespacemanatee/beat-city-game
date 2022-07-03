using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.MMInterface
{
	/// <summary>
	///     A component to handle popups, their opening and closing
	/// </summary>
	public class MMPopup : MonoBehaviour
    {
        /// true if the popup is currently open
        public bool CurrentlyOpen;

        [Header("Fader")] public float FaderOpenDuration = 0.2f;

        public float FaderCloseDuration = 0.2f;
        public float FaderOpacity = 0.8f;
        public MMTweenType Tween = new(MMTween.MMTweenCurve.EaseInCubic);
        public int ID;

        protected Animator _animator;


        /// <summary>
        ///     On Start, we initialize our popup
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        ///     On update, we update our animator parameter
        /// </summary>
        protected virtual void Update()
        {
            if (_animator != null) _animator.SetBool("Closed", !CurrentlyOpen);
        }

        /// <summary>
        ///     On Init, we grab our animator and store it for future use
        /// </summary>
        protected virtual void Initialization()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        ///     Opens the popup
        /// </summary>
        public virtual void Open()
        {
            if (CurrentlyOpen) return;

            MMFadeEvent.Trigger(FaderOpenDuration, FaderOpacity, Tween, ID);
            _animator.SetTrigger("Open");
            CurrentlyOpen = true;
        }

        /// <summary>
        ///     Closes the popup
        /// </summary>
        public virtual void Close()
        {
            if (!CurrentlyOpen) return;

            MMFadeEvent.Trigger(FaderCloseDuration, 0f, Tween, ID);
            _animator.SetTrigger("Close");
            CurrentlyOpen = false;
        }
    }
}