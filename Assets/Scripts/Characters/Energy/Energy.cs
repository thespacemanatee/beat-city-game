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
    public string PlayerId;

    public EnergyChangeEvent(Energy affectedEnergy, float newEnergy, string playerId)
    {
        AffectedEnergy = affectedEnergy;
        NewEnergy = newEnergy;
        PlayerId = playerId;
    }

    static EnergyChangeEvent e;

    public static void Trigger(Energy affectedEnergy, float newEnergy, string playerId)
    {
        e.AffectedEnergy = affectedEnergy;
        e.NewEnergy = newEnergy;
        e.PlayerId = playerId;
        MMEventManager.TriggerEvent(e);
    }
}

/// <summary>
/// An event triggered when energy is dropped
/// </summary>
public struct EnergyDropEvent
{
    public Vector3 Position;
    public int Count;

    public EnergyDropEvent(Vector3 position, int count)
    {
        Position = position;
        Count = count;
    }

    static EnergyDropEvent e;

    public static void Trigger(Vector3 position, int count)
    {
        e.Position = position;
        e.Count = count;
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

    /// the minimum amount of energy of the object
    [Tooltip("the minimum amount of energy of the object")]
    public float MinimumEnergy = -0.01f;

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
    protected virtual void Start()
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
    protected virtual void SetEnergy(float newValue)
    {
        CurrentEnergy = newValue;
        UpdateEnergyBars();
        if (_character != null)
        {
            EnergyChangeEvent.Trigger(this, newValue, _character.PlayerID);
        }
    }

    /// <summary>
    /// Called when the character gets energy (from a energypack for example)
    /// </summary>
    /// <param name="energy">The energy the character gets.</param>
    public virtual void ReceiveEnergy(float energy)
    {
        SetEnergy(Mathf.Min(CurrentEnergy + energy, MaximumEnergy));
        UpdateEnergyBars();
    }

    /// <summary>
    /// Called when the character spends energy (from using abilities for example)
    /// </summary>
    public virtual void SpendEnergy()
    {
        SetEnergy(0);
        UpdateEnergyBars();
    }

    /// <summary>
    /// Called when the character loses energy (from usage or damage)
    /// </summary>
    public virtual void EnergyPenaltyFromDamage()
    {
        var energyPenalty = (int)Mathf.Max(0, CurrentEnergy / 2);
        EnergyDropEvent.Trigger(_character.transform.position, energyPenalty);
        SetEnergy(CurrentEnergy - energyPenalty);
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
    protected virtual void UpdateEnergyBars()
    {
        if (_character == null || _character.CharacterType != Character.CharacterTypes.Player) return;
        // We update the energy bar
        if (BeatCityGUIManager.HasInstance)
        {
            ((BeatCityGUIManager)BeatCityGUIManager.Instance).UpdateEnergyBars(CurrentEnergy, MinimumEnergy,
                MaximumEnergy, _character.PlayerID);
        }
    }

    #endregion
}