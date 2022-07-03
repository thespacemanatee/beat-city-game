using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     This action forces the character to stop crouching if it can
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionCrouchStop")]
    //[RequireComponent(typeof(CharacterCrouch))]
    public class AIActionCrouchStop : AIAction
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
        ///     On PerformAction we stop crouching
        /// </summary>
        public override void PerformAction()
        {
            if (_character == null || _characterCrouch == null) return;

            if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Crouching
                || _character.MovementState.CurrentState == CharacterStates.MovementStates.Crawling)
                _characterCrouch.StopForcedCrouch();
        }
    }
}