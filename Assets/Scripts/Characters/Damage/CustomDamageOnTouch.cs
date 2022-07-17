using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Characters.Damage
{
    public class CustomDamageOnTouch : DamageOnTouch
    {
        // storage
        private Energy _colliderEnergy;

        protected override void Colliding(GameObject collider)
        {
            _colliderEnergy = collider.GetComponent<Energy>();
            base.Colliding(collider);
        }

        protected override void OnCollideWithDamageable(Health health)
        {
            base.OnCollideWithDamageable(health);
            _colliderEnergy.EnergyPenaltyFromDamage();
        }
    }
}