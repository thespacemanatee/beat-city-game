using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     This action forces the character to crouch if it can
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionCrouchStart")]
    //[RequireComponent(typeof(CharacterCrouch))]
    public class AIActionCrouchStart : AIAction
    {
        protected Character _character;
        protected CharacterCrouch _characterCrouch;

        /// <summary>
        ///     Grabs dependencies
        /// </summary>
        public override void Initialization()
        {
            _character = gameObject.GetComponentInParent<Character>();
            _characterCrouch = _character?.FindAbility<CharacterCrouch>();
        }

        /// <summary>
        ///     On PerformAction we crouch
        /// </summary>
        public override void PerformAction()
        {
            if (_character == null || _characterCrouch == null) return;

            if (_character.MovementState.CurrentState != CharacterStates.MovementStates.Crouching
                && _character.MovementState.CurrentState != CharacterStates.MovementStates.Crawling)
                _characterCrouch.StartForcedCrouch();
        }
    }
}