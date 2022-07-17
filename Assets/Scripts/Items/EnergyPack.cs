using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// An energy pack that increases the player's energy count and enables the player to use abilities.
/// </summary>
public class EnergyPack : PickableItem
{
    [Header("EnergyPack")]
    /// The amount of points to add when collected
    [Tooltip("The amount of points to add when collected")]
    public float HealthToGive = 10f;

    /// if this is true, only player characters can pick this up
    [Tooltip("if this is true, only player characters can pick this up")]
    public bool OnlyForPlayerCharacter = true;

    /// <summary>
    /// Triggered when something collides with the energy pack.
    /// </summary>
    /// <param name="picker">Other.</param>
    protected override void Pick(GameObject picker)
    {
        var character = picker.gameObject.MMGetComponentNoAlloc<Character>();
        if (OnlyForPlayerCharacter && (character != null) &&
            (_character.CharacterType != Character.CharacterTypes.Player))
        {
            return;
        }

        var characterHealth = picker.gameObject.MMGetComponentNoAlloc<Health>();
        // else, we give health to the player
        if (characterHealth != null)
        {
            characterHealth.ReceiveHealth(HealthToGive, gameObject);
        }
    }
}