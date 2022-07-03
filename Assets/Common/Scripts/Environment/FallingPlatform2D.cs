using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	///     Add this script to a platform and it'll fall down when walked upon by a playable character
	///     Add an AutoRespawn component to your platform and it'll get reset when your character dies
	/// </summary>
	[AddComponentMenu("TopDown Engine/Environment/Falling Platform 2D")]
    public class FallingPlatform2D : MonoBehaviour
    {
        /// the possible states for the falling platform
        public enum FallingPlatformStates
        {
            Idle,
            Shaking,
            Falling,
            ColliderOff
        }

        /// the current state of the falling platform
        [MMReadOnly] [Tooltip("the current state of the falling platform")]
        public FallingPlatformStates State;

        /// if this is true, the platform will fall inevitably once touched
        [Tooltip("if this is true, the platform will fall inevitably once touched")]
        public bool InevitableFall;

        /// the time (in seconds) before the fall of the platform
        [Tooltip("the time (in seconds) before the fall of the platform")]
        public float TimeBeforeFall = 2f;

        /// the time (in seconds) before the collider turns itself off once the fall has started
        [Tooltip("the time (in seconds) before the collider turns itself off once the fall has started")]
        public float DelayBetweenFallAndColliderOff = 0.5f;

        // private stuff
        protected Animator _animator;
        protected Bounds _bounds;
        protected Collider2D _collider;
        protected bool _contact;
        protected float _fallStartedAt;
        protected Vector3 _initialPosition;
        protected Vector2 _newPosition;
        protected float _timeLeftBeforeFall;

        /// <summary>
        ///     Initialization
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        ///     This is called every frame.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // we send our various states to the animator.		
            UpdateAnimator();

            if (_contact) _timeLeftBeforeFall -= Time.deltaTime;

            if (_timeLeftBeforeFall < 0)
            {
                if (State != FallingPlatformStates.Falling) _fallStartedAt = Time.time;
                State = FallingPlatformStates.Falling;
            }

            if (State == FallingPlatformStates.Falling)
                if (Time.time - _fallStartedAt >= DelayBetweenFallAndColliderOff)
                    _collider.enabled = false;
        }

        /// <summary>
        ///     Triggered when a TopDownController exits the platform
        /// </summary>
        /// <param name="controller">The TopDown controller that collides with the platform.</param>
        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (InevitableFall) return;

            var controller = collider.gameObject.GetComponent<TopDownController>();
            if (controller == null)
                return;

            _contact = false;
            if (State == FallingPlatformStates.Shaking) State = FallingPlatformStates.Idle;
        }

        /// <summary>
        ///     Triggered when a TopDownController touches the platform
        /// </summary>
        /// <param name="controller">The TopDown controller that collides with the platform.</param>
        public virtual void OnTriggerStay2D(Collider2D collider)
        {
            var controller = collider.gameObject.MMGetComponentNoAlloc<TopDownController2D>();
            if (controller == null) return;

            if (State == FallingPlatformStates.Falling) return;

            if (TimeBeforeFall > 0)
            {
                _contact = true;
                State = FallingPlatformStates.Shaking;
            }
            else
            {
                if (!InevitableFall)
                {
                    _contact = false;
                    State = FallingPlatformStates.Idle;
                }
            }
        }

        /// <summary>
        ///     Grabs components and saves initial position and timer
        /// </summary>
        protected virtual void Initialization()
        {
            // we get the animator
            State = FallingPlatformStates.Idle;
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            _collider.enabled = true;
            _bounds = LevelManager.Instance.LevelBounds;
            _initialPosition = transform.position;
            _timeLeftBeforeFall = TimeBeforeFall;
        }

        /// <summary>
        ///     Disables the falling platform. We're not destroying it, so we can revive it on respawn
        /// </summary>
        protected virtual void DisableFallingPlatform()
        {
            gameObject.SetActive(false);
            transform.position = _initialPosition;
            _timeLeftBeforeFall = TimeBeforeFall;
            State = FallingPlatformStates.Idle;
        }

        /// <summary>
        ///     Updates the block's animator.
        /// </summary>
        protected virtual void UpdateAnimator()
        {
            if (_animator != null)
            {
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, "Idle", State == FallingPlatformStates.Idle);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, "Shaking", State == FallingPlatformStates.Shaking);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, "Falling", State == FallingPlatformStates.Falling);
            }
        }

        /// <summary>
        ///     On Revive, we restore this platform's state
        /// </summary>
        protected virtual void OnRevive()
        {
            transform.position = _initialPosition;
            _timeLeftBeforeFall = TimeBeforeFall;
            State = FallingPlatformStates.Idle;
        }
    }
}