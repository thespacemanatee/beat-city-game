using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     An ability that casts a cone of vision around the character.
    /// </summary>
    [RequireComponent(typeof(MMConeOfVision))]
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Cone of Vision")]
    public class CharacterConeOfVision : MonoBehaviour
    {
        protected CharacterOrientation3D _characterOrientation;
        protected MMConeOfVision _coneOfVision;

        /// <summary>
        ///     On awake, we grab our components
        /// </summary>
        protected virtual void Awake()
        {
            _characterOrientation = gameObject.GetComponentInParent<CharacterOrientation3D>();
            _coneOfVision = gameObject.GetComponent<MMConeOfVision>();
        }

        /// <summary>
        ///     On update, we update our cone of vision
        /// </summary>
        protected virtual void Update()
        {
            UpdateDirection();
        }

        /// <summary>
        ///     Sends the character orientation's angle to the cone of vision
        /// </summary>
        protected virtual void UpdateDirection()
        {
            if (_characterOrientation == null)
                _coneOfVision.SetDirectionAndAngles(transform.forward, transform.eulerAngles);
            else
                _coneOfVision.SetDirectionAndAngles(_characterOrientation.ModelDirection,
                    _characterOrientation.ModelAngles);
        }
    }
}