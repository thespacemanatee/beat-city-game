using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CustomCharacterInventory : CharacterInventory
{
    protected override void GrabInventories()
    {
        if (MainInventory == null)
        {
            var mainInventoryTmp = Resources
                .FindObjectsOfTypeAll<Inventory>()
                .FirstOrDefault(obj => obj.name == MainInventoryName && obj.PlayerID == PlayerID);
            if (mainInventoryTmp != null)
            {
                MainInventory = mainInventoryTmp.GetComponent<Inventory>();
            }
        }

        if (WeaponInventory == null)
        {
            var weaponInventoryTmp = Resources
                .FindObjectsOfTypeAll<Inventory>()
                .FirstOrDefault(obj => obj.name == WeaponInventoryName && obj.PlayerID == PlayerID);
            if (weaponInventoryTmp != null)
            {
                WeaponInventory = weaponInventoryTmp.GetComponent<Inventory>();
            }
        }

        if (HotbarInventory == null)
        {
            var hotbarInventoryTmp = Resources
                .FindObjectsOfTypeAll<Inventory>()
                .FirstOrDefault(obj => obj.name == HotbarInventoryName && obj.PlayerID == PlayerID);
            if (hotbarInventoryTmp != null)
            {
                HotbarInventory = hotbarInventoryTmp.GetComponent<Inventory>();
            }
        }

        if (MainInventory != null)
        {
            MainInventory.SetOwner(gameObject);
            MainInventory.TargetTransform = InventoryTransform;
        }

        if (WeaponInventory != null)
        {
            WeaponInventory.SetOwner(gameObject);
            WeaponInventory.TargetTransform = InventoryTransform;
        }

        if (HotbarInventory != null)
        {
            HotbarInventory.SetOwner(gameObject);
            HotbarInventory.TargetTransform = InventoryTransform;
        }

        Debug.Log("MainInventory: " + MainInventory.PlayerID);
    }
}