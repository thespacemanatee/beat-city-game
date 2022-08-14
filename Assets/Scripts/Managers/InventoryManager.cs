using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class InventoryManager : MonoBehaviour, MMEventListener<EnergyChangeEvent>, MMEventListener<WeaponFireEvent>
{
    public MultiplayerLevelManager LevelManager;
    public List<InventoryWeapon> Weapons;

    private CharacterInventory _player1Inventory;
    private CharacterInventory _player2Inventory;
    private CharacterInventory _player3Inventory;
    private CharacterInventory _player4Inventory;
    private Energy _player1Energy;
    private Energy _player2Energy;
    private Energy _player3Energy;
    private Energy _player4Energy;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var character in LevelManager.Players)
        {
            switch (character.PlayerID)
            {
                case "Player1":
                {
                    _player1Inventory = character.GetComponent<CharacterInventory>();
                    _player1Energy = character.GetComponent<Energy>();
                    break;
                }
                case "Player2":
                {
                    _player2Inventory = character.GetComponent<CharacterInventory>();
                    _player2Energy = character.GetComponent<Energy>();
                    break;
                }
                case "Player3":
                {
                    _player3Inventory = character.GetComponent<CharacterInventory>();
                    _player3Energy = character.GetComponent<Energy>();
                    break;
                }
                case "Player4":
                {
                    _player4Inventory = character.GetComponent<CharacterInventory>();
                    _player4Energy = character.GetComponent<Energy>();
                    break;
                }
            }

            foreach (var weapon in Weapons)
            {
                MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, weapon.TargetInventoryName,
                    weapon, 1, 0, character.PlayerID);
            }
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening<EnergyChangeEvent>();
        this.MMEventStartListening<WeaponFireEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<EnergyChangeEvent>();
        this.MMEventStopListening<WeaponFireEvent>();
    }

    public void OnMMEvent(EnergyChangeEvent eventType)
    {
        Debug.Log(eventType.PlayerId + ": " + eventType.NewEnergy);
        var index = (int)eventType.NewEnergy - 1;
        switch (eventType.PlayerId)
        {
            case "Player1":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player1Inventory.WeaponInventory.name, _player1Inventory.WeaponInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, _player1Inventory.MainInventory.name,
                    Weapons[index], 0, 0, eventType.PlayerId);
                break;
            }
            case "Player2":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player2Inventory.WeaponInventory.name, _player2Inventory.WeaponInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, _player2Inventory.MainInventory.name,
                    Weapons[index], 0, 0, eventType.PlayerId);
                break;
            }
            case "Player3":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player3Inventory.WeaponInventory.name, _player3Inventory.WeaponInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, _player3Inventory.MainInventory.name,
                    Weapons[index], 0, 0, eventType.PlayerId);
                break;
            }
            case "Player4":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player4Inventory.WeaponInventory.name, _player4Inventory.WeaponInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, _player4Inventory.MainInventory.name,
                    Weapons[index], 0, 0, eventType.PlayerId);
                break;
            }
        }
    }

    public void OnMMEvent(WeaponFireEvent eventType)
    {
        switch (eventType.PlayerId)
        {
            case "Player1":
            {
                _player1Energy.SpendEnergy();
                break;
            }
            case "Player2":
            {
                _player2Energy.SpendEnergy();
                break;
            }
            case "Player3":
            {
                _player3Energy.SpendEnergy();
                break;
            }
            case "Player4":
            {
                _player4Energy.SpendEnergy();
                break;
            }
        }
    }
}