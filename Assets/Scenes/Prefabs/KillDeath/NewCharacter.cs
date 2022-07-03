using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class NewCharacter : Character
{
    public NewHealth NewCharacterHealth;
    public CustomKDEvent onPlayerDeath;
    private bool dropped=false;
    protected virtual void NewOnDeath(GameObject instigator)
    {
        if (CharacterBrain != null)
        {
            CharacterBrain.TransitionToState("");
            CharacterBrain.enabled = false;
        }
        if (MovementState.CurrentState != CharacterStates.MovementStates.FallingDownHole)
        {
            MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        }
        Debug.Log("New On Death Triggered");
        Debug.Log("Killer:");   
        Debug.Log(instigator.GetComponent<Projectile>().Owner.GetComponent<Character>().PlayerID);    
        string killer = instigator.GetComponent<Projectile>().Owner.GetComponent<Character>().PlayerID;
        Debug.Log("Death:");
        Debug.Log(GetComponent<Character>().PlayerID);
        string victim = GetComponent<Character>().PlayerID;
        onPlayerDeath.Invoke(killer, victim);
    }
    protected override void Update()
    {
        base.Update();
        if (GetComponent<Transform>().position.y < 0.0f && !dropped){
            string victim = GetComponent<Character>().PlayerID;
            onPlayerDeath.Invoke(victim, victim);
            dropped = true;
        }
    }

    /// <summary>
    /// OnEnable, we register our OnRevive event
    /// </summary>
    protected override void OnEnable ()
    {
        if (CharacterHealth != null)
        {
            if (!_onReviveRegistered)
            {
                CharacterHealth.OnRevive += OnRevive;
                _onReviveRegistered = true;
            }
            CharacterHealth.OnDeath += OnDeath;
            NewCharacterHealth.NewOnDeath += NewOnDeath;
            CharacterHealth.OnHit += OnHit;
        }
    }

    /// <summary>
    /// OnDisable, we unregister our OnRevive event
    /// </summary>
    protected override void OnDisable()
    {
        if (CharacterHealth != null)
        {
            CharacterHealth.OnDeath -= OnDeath;
            NewCharacterHealth.NewOnDeath -= NewOnDeath;
            CharacterHealth.OnHit -= OnHit;
        }			
    }
}
