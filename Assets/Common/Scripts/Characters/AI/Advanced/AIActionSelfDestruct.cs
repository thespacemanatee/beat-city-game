using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     An AIACtion used to have an AI kill itself
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionSelfDestruct")]
    public class AIActionSelfDestruct : AIAction
    {
        public bool OnlyRunOnce = true;
        protected bool _alreadyRan;

        protected Character _character;
        protected Health _health;

        /// <summary>
        ///     On init we grab our Health
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            _character = gameObject.GetComponentInParent<Character>();
            _health = _character.CharacterHealth;
        }

        /// <summary>
        ///     Kills the AI
        /// </summary>
        public override void PerformAction()
        {
            if (OnlyRunOnce && _alreadyRan) return;
            _health.Kill();
            _brain.BrainActive = false;
            _alreadyRan = true;
        }

        /// <summary>
        ///     On enter state we reset our flag
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            _alreadyRan = false;
        }
    }
}