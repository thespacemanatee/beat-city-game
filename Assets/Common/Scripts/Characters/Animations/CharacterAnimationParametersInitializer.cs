using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     A struct used to store character animation parameter definitions, to be used by the
    ///     CharacterAnimationParametersInitializer class
    /// </summary>
    public struct TopDownCharacterAnimationParameter
    {
        /// the name of the parameter
        public string ParameterName;

        /// the type of the parameter
        public AnimatorControllerParameterType ParameterType;

        public TopDownCharacterAnimationParameter(string name, AnimatorControllerParameterType type)
        {
            ParameterName = name;
            ParameterType = type;
        }
    }

    /// <summary>
    /// </summary>
    public class CharacterAnimationParametersInitializer : MonoBehaviour
    {
        [Header("Initialization")]
        /// if this is true, this component will remove itself after adding the character parameters
        [Tooltip("if this is true, this component will remove itself after adding the character parameters")]
        public bool AutoRemoveAfterInitialization = true;

        [MMInspectorButton("AddAnimationParameters")]
        public bool AddAnimationParametersButton;

        protected Animator _animator;
#if UNITY_EDITOR
        protected AnimatorController _controller;
#endif
        protected List<string> _parameters = new();

        protected TopDownCharacterAnimationParameter[] ParametersArray =
        {
            new("Alive", AnimatorControllerParameterType.Bool),
            new("Grounded", AnimatorControllerParameterType.Bool),
            new("Idle", AnimatorControllerParameterType.Bool),
            new("Walking", AnimatorControllerParameterType.Bool),
            new("Running", AnimatorControllerParameterType.Bool),
            new("Activating", AnimatorControllerParameterType.Bool),
            new("Crouching", AnimatorControllerParameterType.Bool),
            new("Crawling", AnimatorControllerParameterType.Bool),
            new("Damage", AnimatorControllerParameterType.Trigger),
            new("Dashing", AnimatorControllerParameterType.Bool),
            new("DashingDirectionX", AnimatorControllerParameterType.Float),
            new("DashingDirectionY", AnimatorControllerParameterType.Float),
            new("DashingDirectionZ", AnimatorControllerParameterType.Float),
            new("DashStarted", AnimatorControllerParameterType.Bool),
            new("Death", AnimatorControllerParameterType.Trigger),
            new("FallingDownHole", AnimatorControllerParameterType.Bool),
            new("WeaponEquipped", AnimatorControllerParameterType.Bool),
            new("WeaponEquippedID", AnimatorControllerParameterType.Int),
            new("Jumping", AnimatorControllerParameterType.Bool),
            new("HitTheGround", AnimatorControllerParameterType.Bool),
            new("Random", AnimatorControllerParameterType.Float),
            new("RandomConstant", AnimatorControllerParameterType.Int),
            new("Direction", AnimatorControllerParameterType.Float),
            new("Speed", AnimatorControllerParameterType.Float),
            new("xSpeed", AnimatorControllerParameterType.Float),
            new("ySpeed", AnimatorControllerParameterType.Float),
            new("zSpeed", AnimatorControllerParameterType.Float),
            new("HorizontalDirection", AnimatorControllerParameterType.Float),
            new("VerticalDirection", AnimatorControllerParameterType.Float),
            new("RelativeForwardSpeed", AnimatorControllerParameterType.Float),
            new("RelativeLateralSpeed", AnimatorControllerParameterType.Float),
            new("RelativeForwardSpeedNormalized", AnimatorControllerParameterType.Float),
            new("RelativeLateralSpeedNormalized", AnimatorControllerParameterType.Float),
            new("RemappedForwardSpeedNormalized", AnimatorControllerParameterType.Float),
            new("RemappedLateralSpeedNormalized", AnimatorControllerParameterType.Float),
            new("RemappedSpeedNormalized", AnimatorControllerParameterType.Float),
            new("YRotationSpeed", AnimatorControllerParameterType.Float)
        };

        /// <summary>
        ///     Adds all the default animation parameters on your character's animator
        /// </summary>
        public virtual void AddAnimationParameters()
        {
            // we grab the animator
            _animator = gameObject.GetComponent<Animator>();
            if (_animator == null)
                Debug.LogError(
                    "You need to add the AnimationParameterInitializer class to a gameobject with an Animator.");

            // we grab the controller
#if UNITY_EDITOR
            _controller = _animator.runtimeAnimatorController as AnimatorController;
            if (_controller == null) Debug.LogError("You need an animator controller on this Animator.");
#endif

            // we store its parameters
            _parameters.Clear();
            foreach (var param in _animator.parameters) _parameters.Add(param.name);

            // we add all the listed parameters
            foreach (var parameter in ParametersArray)
                if (!_parameters.Contains(parameter.ParameterName))
                {
#if UNITY_EDITOR
                    _controller.AddParameter(parameter.ParameterName, parameter.ParameterType);
#endif
                }

            // we remove this component if needed
            if (AutoRemoveAfterInitialization) DestroyImmediate(this);
        }
    }
}