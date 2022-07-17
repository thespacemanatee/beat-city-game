using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// An event triggered every time energy values change, for other classes to listen to
/// </summary>
public struct EnergyChangeEvent
{
    public Energy AffectedEnergy;
    public float NewEnergy;

    public EnergyChangeEvent(Energy affectedEnergy, float newEnergy)
    {
        AffectedEnergy = affectedEnergy;
        NewEnergy = newEnergy;
    }

    static EnergyChangeEvent e;

    public static void Trigger(Energy affectedEnergy, float newEnergy)
    {
        e.AffectedEnergy = affectedEnergy;
        e.NewEnergy = newEnergy;
        MMEventManager.TriggerEvent(e);
    }
}

/// <summary>
/// This class manages the energy of an object, pilots its potential energy bar, handles what happens when it takes damage,
/// and what happens when it dies.
/// </summary>
public class Energy : MMMonoBehaviour
{
    [MMInspectorGroup("Status", true, 29)]
    /// the current energy of the character
    [MMReadOnly]
    [Tooltip("the current energy of the character")]
    public float CurrentEnergy;

    [MMInspectorGroup("Energy", true, 5)]
    [MMInformation(
        "Add this component to an object and it'll have energy.",
        MMInformationAttribute.InformationType.Info, false)]
    /// the initial amount of energy of the object
    [Tooltip("the initial amount of energy of the object")]
    public float InitialEnergy;

    /// the maximum amount of energy of the object
    [Tooltip("the maximum amount of energy of the object")]
    public float MaximumEnergy = 7;

    /// if this is true, energy values will be reset everytime this character is enabled (usually at the start of a scene)
    [Tooltip(
        "if this is true, energy values will be reset everytime this character is enabled (usually at the start of a scene)")]
    public bool ResetEnergyOnEnable = true;

    protected Character _character;

    #region Initialization

    /// <summary>
    /// On Start, we initialize our energy
    /// </summary>
    protected virtual void Awake()
    {
        Initialization();
        InitializeCurrentEnergy();
    }

    /// <summary>
    /// Grabs useful components, enables damage and gets the inital color
    /// </summary>
    public virtual void Initialization()
    {
        _character = gameObject.GetComponentInParent<Character>();
    }

    /// <summary>
    /// Initializes energy to either initial or current values
    /// </summary>
    public virtual void InitializeCurrentEnergy()
    {
        SetEnergy(InitialEnergy);
    }

    /// <summary>
    /// When the object is enabled (on respawn for example), we restore its initial energy levels
    /// </summary>
    protected virtual void OnEnable()
    {
        if (ResetEnergyOnEnable)
        {
            InitializeCurrentEnergy();
        }
    }

    /// <summary>
    /// On Disable, we prevent any delayed destruction from running
    /// </summary>
    protected virtual void OnDisable()
    {
        CancelInvoke();
    }

    #endregion

    #region EnergyManipulationAPIs

    /// <summary>
    /// Sets the current energy to the specified new value, and updates the energy bar
    /// </summary>
    /// <param name="newValue"></param>
    public virtual void SetEnergy(float newValue)
    {
        CurrentEnergy = newValue;
        UpdateEnergyBars();
        EnergyChangeEvent.Trigger(this, newValue);
    }

    /// <summary>
    /// Called when the character gets energy (from a stimpack for example)
    /// </summary>
    /// <param name="energy">The energy the character gets.</param>
    /// <param name="instigator">The thing that gives the character energy.</param>
    public virtual void ReceiveEnergy(float energy)
    {
        SetEnergy(Mathf.Min(CurrentEnergy + energy, MaximumEnergy));
        UpdateEnergyBars();
    }

    /// <summary>
    /// Resets the character's energy to its max value
    /// </summary>
    public virtual void ResetEnergyToMaxEnergy()
    {
        SetEnergy(MaximumEnergy);
    }

    /// <summary>
    /// Forces a refresh of the character's energy bar
    /// </summary>
    public virtual void UpdateEnergyBars()
    {
        if (_character != null)
        {
            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                // We update the energy bar
                if (BeatCityGUIManager.HasInstance)
                {
                    ((BeatCityGUIManager)BeatCityGUIManager.Instance).UpdateEnergyBars(CurrentEnergy, -0.01f,
                        MaximumEnergy, _character.PlayerID);
                }
            }
        }
    }

    #endregion
}