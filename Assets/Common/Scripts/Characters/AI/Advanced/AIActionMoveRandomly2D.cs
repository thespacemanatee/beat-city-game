using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     Requires a CharacterMovement ability.
    ///     Makes the character move randomly, until it finds an obstacle in its path, in which case it'll pick a new direction
    ///     at random
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveRandomly2D")]
    //[RequireComponent(typeof(CharacterMovement))]
    public class AIActionMoveRandomly2D : AIAction
    {
        [Header("Duration")]
        /// the maximum time a character can spend going in a direction without changing
        [Tooltip("the maximum time a character can spend going in a direction without changing")]
        public float MaximumDurationInADirection = 2f;

        [Header("Obstacles")]
        /// the layers the character will try to avoid
        [Tooltip("the layers the character will try to avoid")]
        public LayerMask ObstacleLayerMask = LayerManager.ObstaclesLayerMask;

        /// the minimum distance from the target this Character can reach.
        [Tooltip("the minimum distance from the target this Character can reach.")]
        public float ObstaclesDetectionDistance = 1f;

        /// the frequency (in seconds) at which to check for obstacles
        [Tooltip("the frequency (in seconds) at which to check for obstacles")]
        public float ObstaclesCheckFrequency = 1f;

        /// the minimal random direction to randomize from
        [Tooltip("the minimal random direction to randomize from")]
        public Vector2 MinimumRandomDirection = new(-1f, -1f);

        /// the maximum random direction to randomize from
        [Tooltip("the maximum random direction to randomize from")]
        public Vector2 MaximumRandomDirection = new(1f, 1f);

        protected CharacterMovement _characterMovement;
        protected Collider2D _collider;
        protected Vector2 _direction;
        protected float _lastDirectionChangeTimestamp;
        protected float _lastObstacleDetectionTimestamp;

        /// <summary>
        ///     On start we grab our character movement component and pick a random direction
        /// </summary>
        public override void Initialization()
        {
            _characterMovement = gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
            _collider = gameObject.GetComponentInParent<Collider2D>();
            PickRandomDirection();
        }

        /// <summary>
        ///     On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            CheckForObstacles();
            CheckForDuration();
            Move();
        }

        /// <summary>
        ///     Moves the character
        /// </summary>
        protected virtual void Move()
        {
            _characterMovement.SetMovement(_direction);
        }

        /// <summary>
        ///     Checks for obstacles by casting a ray
        /// </summary>
        protected virtual void CheckForObstacles()
        {
            if (Time.time - _lastObstacleDetectionTimestamp < ObstaclesCheckFrequency) return;

            var hit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, _direction.normalized,
                _direction.magnitude, ObstacleLayerMask);
            if (hit) PickRandomDirection();

            _lastObstacleDetectionTimestamp = Time.time;
        }

        /// <summary>
        ///     Checks whether or not we should pick a new direction at random
        /// </summary>
        protected virtual void CheckForDuration()
        {
            if (Time.time - _lastDirectionChangeTimestamp > MaximumDurationInADirection) PickRandomDirection();
        }

        /// <summary>
        ///     Picks a random direction
        /// </summary>
        protected virtual void PickRandomDirection()
        {
            _direction.x = Random.Range(MinimumRandomDirection.x, MaximumRandomDirection.x);
            _direction.y = Random.Range(MinimumRandomDirection.y, MaximumRandomDirection.y);
            _lastDirectionChangeTimestamp = Time.time;
        }

        /// <summary>
        ///     On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement?.SetHorizontalMovement(0f);
            _characterMovement?.SetVerticalMovement(0f);
        }
    }
}