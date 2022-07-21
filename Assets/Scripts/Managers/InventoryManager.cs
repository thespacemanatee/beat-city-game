using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class InventoryManager : MonoBehaviour, MMEventListener<EnergyChangeEvent>
{
    public InventoryItem Weapon;
    public InventoryItem Ammo;

    // Start is called before the first frame update
    private void Start()
    {
        for (var i = 0; i < 4; i++)
        {
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon.TargetInventoryName, Weapon, 1, 0,
                $"Player{i}");
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Ammo.TargetInventoryName, Ammo, 20, 1,
                $"Player{i}");
        }
    }
    
    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnMMEvent(EnergyChangeEvent eventType)
    {
        if (eventType.NewEnergy > 0)
        {
            MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, Weapon.TargetInventoryName, Weapon,
                1, 0, eventType.PlayerId);
        }
    }
}