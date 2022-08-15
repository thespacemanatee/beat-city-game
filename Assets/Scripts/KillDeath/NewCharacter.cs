using MoreMountains.TopDownEngine;
using System;
using UnityEngine;

public class NewCharacter : Character
{
    public NewHealth NewCharacterHealth;

    public CustomKDEvent onPlayerDeath;

    /// <summary>
    ///     OnEnable, we register our OnRevive event
    /// </summary>

    protected void Start()
    {
        if (NewCharacterHealth != null)
        {
            NewCharacterHealth.NewOnDeath += NewOnDeath;
        }
        else
        {
            Debug.Log("NEW CHARACTER HEALTH IS NOT ENABLED");
        }
    }

    protected override void OnEnable()
    {
        if (NewCharacterHealth != null)
        {
            if (!_onReviveRegistered)
            {
                CharacterHealth.OnRevive += OnRevive;
                _onReviveRegistered = true;
            }

            //CharacterHealth.OnDeath += OnDeath;
            NewCharacterHealth.NewOnDeath += NewOnDeath;
            CharacterHealth.OnHit += OnHit;
        }
    }

    /// <summary>
    ///     OnDisable, we unregister our OnRevive event
    /// </summary>
    protected override void OnDisable()
    {
        if (NewCharacterHealth != null)
        {
            //CharacterHealth.OnDeath -= OnDeath;
            NewCharacterHealth.NewOnDeath -= NewOnDeath;
            CharacterHealth.OnHit -= OnHit;
        }
    }

    protected virtual void NewOnDeath(GameObject instigator)
    {
        Debug.Log("New On Death Triggered");
        if (CharacterBrain != null)
        {
            CharacterBrain.TransitionToState("");
            CharacterBrain.enabled = false;
        }

        if (MovementState.CurrentState != CharacterStates.MovementStates.FallingDownHole)
            MovementState.ChangeState(CharacterStates.MovementStates.Idle);

        Debug.Log("Killer:");
        var killer = "null";
        try
        {
            killer = instigator.GetComponent<MeleeWeapon>().Owner.GetComponent<Character>().PlayerID;
        }
        catch (NullReferenceException)
        {
            try
            {
                killer = instigator.GetComponent<Projectile>().Owner.GetComponent<Character>().PlayerID;
            }
            catch (NullReferenceException)
            {
                try
                {
                    killer = instigator.GetComponent<HitscanWeapon>().Owner.GetComponent<Character>().PlayerID;
                }
                catch (NullReferenceException)
                {
                    try
                    {
                        killer = instigator.transform.parent.gameObject.GetComponent<PhysicsProjectile>().Owner.GetComponent<Character>().PlayerID;
                    }
                    catch(NullReferenceException)
                    {
                        try
                        {
                            killer = GetComponent<Character>().PlayerID;
                        }
                        catch (NullReferenceException)
                        {
                            Debug.Log("WRONG INSTIGATOR RECEIVED");
                        }
                    }
                }
            }
        }
        Debug.Log(killer);
        var victim = GetComponent<Character>().PlayerID;
        if (killer != "null")
        {
            onPlayerDeath.Invoke(killer, victim);
        }
    }
}