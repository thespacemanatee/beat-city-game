using System.Collections;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class InventoryManager : MonoBehaviour, MMEventListener<EnergyChangeEvent>
{
    public MultiplayerLevelManager LevelManager;
    public InventoryWeapon Weapon;
    public InventoryItem Ammo;

    private CharacterInventory _player1Inventory;
    private CharacterInventory _player2Inventory;
    private CharacterInventory _player3Inventory;
    private CharacterInventory _player4Inventory;
    private CharacterHandleWeapon _player1WeaponHandler;
    private CharacterHandleWeapon _player2WeaponHandler;
    private CharacterHandleWeapon _player3WeaponHandler;
    private CharacterHandleWeapon _player4WeaponHandler;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var character in LevelManager.Players)
        {
            if (character.PlayerID == "Player1")
            {
                _player1Inventory = character.GetComponent<CharacterInventory>();
                _player1WeaponHandler = character.GetComponent<CharacterHandleWeapon>();
            }

            if (character.PlayerID == "Player2")
            {
                _player2Inventory = character.GetComponent<CharacterInventory>();
                _player2WeaponHandler = character.GetComponent<CharacterHandleWeapon>();
            }

            if (character.PlayerID == "Player3")
            {
                _player3Inventory = character.GetComponent<CharacterInventory>();
                _player3WeaponHandler = character.GetComponent<CharacterHandleWeapon>();
            }

            if (character.PlayerID == "Player4")
            {
                _player4Inventory = character.GetComponent<CharacterInventory>();
                _player4WeaponHandler = character.GetComponent<CharacterHandleWeapon>();
            }
        }

        for (var i = 0; i < 4; i++)
        {
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Weapon.TargetInventoryName, Weapon, 1, 0,
                $"Player{i}");
            MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, Ammo.TargetInventoryName, Ammo, 999, 1,
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
            switch (eventType.PlayerId)
            {
                case "Player1":
                {
                    _player1WeaponHandler.ChangeWeapon(Weapon.EquippableWeapon, Weapon.EquippableWeapon.WeaponName);
                    MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                        _player1Inventory.MainInventory.name, _player1Inventory.MainInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }
                case "Player2":
                {
                    _player2WeaponHandler.ChangeWeapon(Weapon.EquippableWeapon, Weapon.EquippableWeapon.WeaponName);
                    MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                        _player2Inventory.MainInventory.name, _player2Inventory.MainInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }
                case "Player3":
                {
                    _player3WeaponHandler.ChangeWeapon(Weapon.EquippableWeapon, Weapon.EquippableWeapon.WeaponName);
                    MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                        _player3Inventory.MainInventory.name, _player3Inventory.MainInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }
                case "Player4":
                {
                    _player4WeaponHandler.ChangeWeapon(Weapon.EquippableWeapon, Weapon.EquippableWeapon.WeaponName);
                    MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null,
                        _player4Inventory.MainInventory.name, _player4Inventory.MainInventory.Content[0], 0, 0,
                        eventType.PlayerId);
                    break;
                }
            }
        }
    }
}