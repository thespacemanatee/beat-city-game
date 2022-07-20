using MoreMountains.TopDownEngine;
using UnityEngine;

public class NewCharacter : Character
{
    public NewHealth NewCharacterHealth;

    public CustomKDEvent onPlayerDeath;

    /// <summary>
    ///     OnEnable, we register our OnRevive event
    /// </summary>
    protected override void OnEnable()
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
    ///     OnDisable, we unregister our OnRevive event
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

    protected virtual void NewOnDeath(GameObject instigator)
    {
        if (CharacterBrain != null)
        {
            CharacterBrain.TransitionToState("");
            CharacterBrain.enabled = false;
        }

        if (MovementState.CurrentState != CharacterStates.MovementStates.FallingDownHole)
            MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        Debug.Log("New On Death Triggered");
        Debug.Log("Killer:");
        Debug.Log(instigator.GetComponent<Projectile>().Owner.GetComponent<Character>().PlayerID);
        var killer = instigator.GetComponent<Projectile>().Owner.GetComponent<Character>().PlayerID;
        Debug.Log("Death:");
        Debug.Log(GetComponent<Character>().PlayerID);
        var victim = GetComponent<Character>().PlayerID;
        onPlayerDeath.Invoke(killer, victim);
    }
}