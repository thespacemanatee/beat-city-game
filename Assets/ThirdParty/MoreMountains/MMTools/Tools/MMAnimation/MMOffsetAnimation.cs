using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Use this class to offset an animation by a random range
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("More Mountains/Tools/Animation/MMOffsetAnimation")]
    public class MMOffsetAnimation : MonoBehaviour
    {
        /// the minimum amount (in seconds) by which to offset the animation
        public float MinimumRandomRange;

        /// the maximum amount (in seconds) by which to offset the animation
        public float MaximumRandomRange = 1f;

        /// the layer to affect
        public int AnimationLayerID;

        /// whether or not to apply that offset on Start
        public bool OffsetOnStart = true;

        /// whether or not to self disable after offsetting
        public bool DisableAfterOffset = true;

        protected Animator _animator;
        protected AnimatorStateInfo _stateInfo;

        /// <summary>
        ///     On awake we store our animator
        /// </summary>
        protected virtual void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        /// <summary>
        ///     On Start we offset our animation
        /// </summary>
        protected virtual void Start()
        {
            OffsetCurrentAnimation();
        }

        /// <summary>
        ///     offsets the target animation
        /// </summary>
        public virtual void OffsetCurrentAnimation()
        {
            if (!OffsetOnStart) return;
            _stateInfo = _animator.GetCurrentAnimatorStateInfo(AnimationLayerID);
            _animator.Play(_stateInfo.fullPathHash, -1, Random.Range(MinimumRandomRange, MaximumRandomRange));
            if (DisableAfterOffset) enabled = false;
        }
    }
}