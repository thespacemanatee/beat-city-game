using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NewHealth : Health
{
    // New death delegate
    public delegate void NewOnDeathDelegate(GameObject instigator);
    public NewOnDeathDelegate NewOnDeath;
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
                MasterHealth.Kill();
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

    /// <summary>
        /// NewKill function to support helper functions
        /// </summary>
    public virtual void NewKill(GameObject instigator)
    {
        Debug.Log("New Kill Triggered");

        if (ImmuneToDamage)
        {
            return;
        }

        if (_character != null)
        {
            // we set its dead state to true
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
            _character.Reset();

            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                TopDownEngineEvent.Trigger(TopDownEngineEventTypes.PlayerDeath, _character);
            }
        }
        SetHealth(0);   

        // we prevent further damage
        DamageDisabled();

        DeathMMFeedbacks?.PlayFeedbacks(this.transform.position);
        NewOnDeath(instigator);
        // Adds points if needed.
        if(PointsWhenDestroyed != 0)
        {
            // we send a new points event for the GameManager to catch (and other classes that may listen to it too)
            TopDownEnginePointEvent.Trigger(PointsMethods.Add, PointsWhenDestroyed);
        }

        if (TargetAnimator != null)
        {
            TargetAnimator.SetTrigger("Death");
        }
        // we make it ignore the collisions from now on
        if (DisableCollisionsOnDeath)
        {
            if (_collider2D != null)
            {
                _collider2D.enabled = false;
            }
            if (_collider3D != null)
            {
                _collider3D.enabled = false;
            }

            // if we have a controller, removes collisions, restores parameters for a potential respawn, and applies a death force
            if (_controller != null)
            {				
                _controller.CollisionsOff();						
            }

            if (DisableChildCollisionsOnDeath)
            {
                foreach (Collider2D collider in this.gameObject.GetComponentsInChildren<Collider2D>())
                {
                    collider.enabled = false;
                }
                foreach (Collider collider in this.gameObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
            }
        }

        
        MMLifeCycleEvent.Trigger(this, MMLifeCycleEventTypes.Death);

        if (DisableControllerOnDeath && (_controller != null))
        {
            _controller.enabled = false;
        }

        if (DisableControllerOnDeath && (_characterController != null))
        {
            _characterController.enabled = false;
        }

        if (DisableModelOnDeath && (Model != null))
        {
            Model.SetActive(false);
        }

        if (DelayBeforeDestruction > 0f)
        {
            Invoke ("DestroyObject", DelayBeforeDestruction);
        }
        else
        {
            // finally we destroy the object
            DestroyObject();	
        }
    }
}