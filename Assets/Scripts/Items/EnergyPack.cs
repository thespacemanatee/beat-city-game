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
    public float EnergyToGive = 1f;

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

        var characterEnergy = picker.gameObject.MMGetComponentNoAlloc<Energy>();
        // else, we give health to the player
        if (characterEnergy != null)
        {
            characterEnergy.ReceiveEnergy(EnergyToGive);
        }
    }

    void Update()
    {
        // Return energy pack to pooler when it falls
        if (gameObject.activeSelf && gameObject.transform.position.y < -10)
        {
            gameObject.SetActive(false);
        }
    }
}