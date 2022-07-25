using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class InventoryManager : MonoBehaviour, MMEventListener<EnergyChangeEvent>, MMEventListener<WeaponFireEvent>
{
    public MultiplayerLevelManager LevelManager;
    public InventoryWeapon Weapon1;
    public InventoryWeapon Weapon2;
    public InventoryWeapon Weapon3;
    public InventoryWeapon Weapon4;
    public InventoryWeapon Weapon5;
    public InventoryWeapon Weapon6;
    public InventoryWeapon Weapon7;

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
            if (character.PlayerID == "Player1")
            {
                _player1Inventory = character.GetComponent<CharacterInventory>();
                _player1Energy = character.GetComponent<Energy>();
            }

            if (character.PlayerID == "Player2")
            {
                _player2Inventory = character.GetComponent<CharacterInventory>();
                _player2Energy = character.GetComponent<Energy>();
            }

            if (character.PlayerID == "Player3")
            {
                _player3Inventory = character.GetComponent<CharacterInventory>();
                _player3Energy = character.GetComponent<Energy>();
            }

            if (character.PlayerID == "Player4")
            {
                _player4Inventory = character.GetComponent<CharacterInventory>();
                _player4Energy = character.GetComponent<Energy>();
            }

            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon1.TargetInventoryName,
                Weapon1, 1, 0, character.PlayerID);
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon2.TargetInventoryName,
                Weapon2, 1, 0, character.PlayerID);
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon3.TargetInventoryName,
                Weapon3, 1, 0, character.PlayerID);
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon4.TargetInventoryName,
                Weapon4, 1, 0, character.PlayerID);
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon5.TargetInventoryName,
                Weapon5, 1, 0, character.PlayerID);
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon6.TargetInventoryName,
                Weapon6, 1, 0, character.PlayerID);
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon7.TargetInventoryName,
                Weapon7, 1, 0, character.PlayerID);
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
        var index = (int)eventType.NewEnergy - 1;
        switch (eventType.PlayerId)
        {
            case "Player1":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player1Inventory.WeaponInventory.name,
                        _player1Inventory.WeaponInventory.Content[0], 0, 0, eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                    _player1Inventory.MainInventory.name, _player1Inventory.MainInventory.Content[index], 0, index,
                    eventType.PlayerId);
                break;
            }
            case "Player2":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player2Inventory.WeaponInventory.name,
                        _player2Inventory.WeaponInventory.Content[0], 0, 0, eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                    _player2Inventory.MainInventory.name, _player2Inventory.MainInventory.Content[index], 0, index,
                    eventType.PlayerId);
                break;
            }
            case "Player3":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player3Inventory.WeaponInventory.name,
                        _player3Inventory.WeaponInventory.Content[0], 0, 0, eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                    _player3Inventory.MainInventory.name, _player3Inventory.MainInventory.Content[index], 0,
                    (int)eventType.NewEnergy - 1,
                    eventType.PlayerId);
                break;
            }
            case "Player4":
            {
                if (index < 0)
                {
                    MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null,
                        _player4Inventory.WeaponInventory.name,
                        _player4Inventory.WeaponInventory.Content[0], 0, 0, eventType.PlayerId);
                    break;
                }

                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                    _player4Inventory.MainInventory.name, _player4Inventory.MainInventory.Content[index], 0, index,
                    eventType.PlayerId);
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