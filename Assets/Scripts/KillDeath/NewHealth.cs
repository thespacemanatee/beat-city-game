using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NewHealth : Health
{
    /// <summary>
    ///     Called when the object takes damage
    ///     Edited health to add in instigator into NewKill
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">
    ///     The time (in seconds) the object should flicker after taking the damage - not used
    ///     anymore, kept to not break retrocompatibility
    /// </param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public override void Damage(float damage, GameObject instigator, float flickerDuration, float invincibilityDuration,
        Vector3 damageDirection, List<TypedDamage> typedDamages = null)
    {
        // if the object is invulnerable, we do nothing and exit
        if (Invulnerable || ImmuneToDamage) return;

        if (!enabled) return;

        // if we're already below zero, we do nothing and exit
        if (CurrentHealth <= 0 && InitialHealth != 0) return;

        // we decrease the character's health by the damage
        var previousHealth = CurrentHealth;
        if (MasterHealth != null)
        {
            previousHealth = MasterHealth.CurrentHealth;
            MasterHealth.SetHealth(MasterHealth.CurrentHealth - damage);
        }
        else
        {
            SetHealth(CurrentHealth - damage);
        }

        LastDamage = damage;
        LastDamageDirection = damageDirection;
        if (OnHit != null) OnHit();

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabled(invincibilityDuration));
        }

        // we trigger a damage taken event
        MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        if (TargetAnimator != null) TargetAnimator.SetTrigger("Damage");

        if (FeedbackIsProportionalToDamage)
            DamageMMFeedbacks?.PlayFeedbacks(transform.position, damage);
        else
            DamageMMFeedbacks?.PlayFeedbacks(transform.position);

        // we update the health bar
        UpdateHealthBar(true);

        // if health has reached zero we set its health to zero (useful for the healthbar)
        if (MasterHealth != null)
        {
            if (MasterHealth.CurrentHealth <= 0)
            {
                MasterHealth.CurrentHealth = 0;
                MasterHealth.NewKill(instigator);
            }
        }
        else
        {
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Destroy(gameObject);
                NewKill(instigator);
            }
        }
    }
}