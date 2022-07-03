using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    [RequireComponent(typeof(Weapon))]
    [AddComponentMenu("TopDown Engine/Weapons/Weapon Ammo")]
    public class WeaponAmmo : MonoBehaviour, MMEventListener<MMStateChangeEvent<Weapon.WeaponStates>>,
        MMEventListener<MMInventoryEvent>, MMEventListener<MMGameEvent>
    {
        [Header("Ammo")]
        /// the ID of this ammo, to be matched on the ammo display if you use one
        [Tooltip("the ID of this ammo, to be matched on the ammo display if you use one")]
        public string AmmoID;

        /// the name of the inventory where the system should look for ammo
        [Tooltip("the name of the inventory where the system should look for ammo")]
        public string AmmoInventoryName = "MainInventory";

        /// the theoretical maximum of ammo
        [Tooltip("the theoretical maximum of ammo")]
        public int MaxAmmo = 100;

        /// if this is true, everytime you equip this weapon, it'll auto fill with ammo
        [Tooltip("if this is true, everytime you equip this weapon, it'll auto fill with ammo")]
        public bool ShouldLoadOnStart = true;

        /// if this is true, everytime you equip this weapon, it'll auto fill with ammo
        [Tooltip("if this is true, everytime you equip this weapon, it'll auto fill with ammo")]
        public bool ShouldEmptyOnSave = true;

        /// the current amount of ammo available in the inventory
        [MMReadOnly] [Tooltip("the current amount of ammo available in the inventory")]
        public int CurrentAmmoAvailable;

        protected InventoryItem _ammoItem;
        protected bool _emptied;

        protected Weapon _weapon;

        /// the inventory where ammo for this weapon is stored
        public Inventory AmmoInventory { get; set; }

        /// <summary>
        ///     On start, we grab the ammo inventory if we can find it
        /// </summary>
        protected virtual void Start()
        {
            var ammoInventoryTmp = GameObject.Find(AmmoInventoryName);
            if (ammoInventoryTmp != null) AmmoInventory = ammoInventoryTmp.GetComponent<Inventory>();
            _weapon = GetComponent<Weapon>();
            if (ShouldLoadOnStart) LoadOnStart();
        }

        /// <summary>
        ///     On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMStateChangeEvent<Weapon.WeaponStates>>();
            this.MMEventStartListening<MMInventoryEvent>();
            this.MMEventStartListening<MMGameEvent>();
        }

        /// <summary>
        ///     On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of
        ///     events.
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMStateChangeEvent<Weapon.WeaponStates>>();
            this.MMEventStopListening<MMInventoryEvent>();
            this.MMEventStartListening<MMGameEvent>();
        }

        protected void OnDestroy()
        {
            // on destroy we put our ammo back in the inventory
            EmptyMagazine();
        }

        /// <summary>
        ///     Grabs inventory events and refreshes ammo if needed
        /// </summary>
        /// <param name="inventoryEvent"></param>
        public virtual void OnMMEvent(MMGameEvent gameEvent)
        {
            switch (gameEvent.EventName)
            {
                case "Save":
                    if (ShouldEmptyOnSave) EmptyMagazine();
                    break;
            }
        }

        /// <summary>
        ///     Grabs inventory events and refreshes ammo if needed
        /// </summary>
        /// <param name="inventoryEvent"></param>
        public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            switch (inventoryEvent.InventoryEventType)
            {
                case MMInventoryEventType.Pick:
                    if (inventoryEvent.EventItem.ItemClass == ItemClasses.Ammo) RefreshCurrentAmmoAvailable();
                    break;
            }
        }

        /// <summary>
        ///     When getting weapon events, we either consume ammo or refill it
        /// </summary>
        /// <param name="weaponEvent"></param>
        public virtual void OnMMEvent(MMStateChangeEvent<Weapon.WeaponStates> weaponEvent)
        {
            // if this event doesn't concern us, we do nothing and exit
            if (weaponEvent.Target != gameObject) return;

            switch (weaponEvent.NewState)
            {
                case Weapon.WeaponStates.WeaponUse:
                    ConsumeAmmo();
                    break;

                case Weapon.WeaponStates.WeaponReloadStop:
                    FillWeaponWithAmmo();
                    break;
            }
        }

        /// <summary>
        ///     Loads our weapon with ammo
        /// </summary>
        protected virtual void LoadOnStart()
        {
            FillWeaponWithAmmo();
        }

        /// <summary>
        ///     Updates the CurrentAmmoAvailable counter
        /// </summary>
        protected virtual void RefreshCurrentAmmoAvailable()
        {
            CurrentAmmoAvailable = AmmoInventory.GetQuantity(AmmoID);
        }

        /// <summary>
        ///     Returns true if this weapon has enough ammo to fire, false otherwise
        /// </summary>
        /// <returns></returns>
        public virtual bool EnoughAmmoToFire()
        {
            if (AmmoInventory == null)
            {
                Debug.LogWarning(name +
                                 " couldn't find the associated inventory. Is there one present in the scene? It should be named '" +
                                 AmmoInventoryName + "'.");
                return false;
            }

            RefreshCurrentAmmoAvailable();

            if (_weapon.MagazineBased)
            {
                if (_weapon.CurrentAmmoLoaded >= _weapon.AmmoConsumedPerShot)
                    return true;
                return false;
            }

            if (CurrentAmmoAvailable >= _weapon.AmmoConsumedPerShot)
                return true;
            return false;
        }

        /// <summary>
        ///     Consumes ammo based on the amount of ammo to consume per shot
        /// </summary>
        protected virtual void ConsumeAmmo()
        {
            if (_weapon.MagazineBased)
                _weapon.CurrentAmmoLoaded = _weapon.CurrentAmmoLoaded - _weapon.AmmoConsumedPerShot;
            else
                for (var i = 0; i < _weapon.AmmoConsumedPerShot; i++)
                {
                    AmmoInventory.UseItem(AmmoID);
                    CurrentAmmoAvailable--;
                }

            if (CurrentAmmoAvailable < _weapon.AmmoConsumedPerShot)
                if (_weapon.AutoDestroyWhenEmpty)
                    StartCoroutine(_weapon.WeaponDestruction());
        }

        /// <summary>
        ///     Fills the weapon with ammo
        /// </summary>
        public virtual void FillWeaponWithAmmo()
        {
            if (AmmoInventory != null) RefreshCurrentAmmoAvailable();

            if (_ammoItem == null)
            {
                var list = AmmoInventory.InventoryContains(AmmoID);
                if (list.Count > 0) _ammoItem = AmmoInventory.Content[list[list.Count - 1]].Copy();
            }

            if (_weapon.MagazineBased)
            {
                var counter = 0;
                var stock = CurrentAmmoAvailable - _weapon.CurrentAmmoLoaded;

                for (var i = _weapon.CurrentAmmoLoaded; i < _weapon.MagazineSize; i++)
                    if (stock > 0)
                    {
                        stock--;
                        counter++;

                        AmmoInventory.UseItem(AmmoID);
                    }

                _weapon.CurrentAmmoLoaded += counter;
            }

            RefreshCurrentAmmoAvailable();
        }

        /// <summary>
        ///     Empties the weapon's magazine and puts the ammo back in the inventory
        /// </summary>
        public virtual void EmptyMagazine()
        {
            if (AmmoInventory != null) RefreshCurrentAmmoAvailable();

            if (_ammoItem == null || AmmoInventory == null) return;

            if (_emptied) return;

            if (_weapon.MagazineBased)
            {
                var stock = _weapon.CurrentAmmoLoaded;
                var counter = 0;

                for (var i = 0; i < stock; i++)
                {
                    AmmoInventory.AddItem(_ammoItem, 1);
                    counter++;
                }

                _weapon.CurrentAmmoLoaded -= counter;

                if (AmmoInventory.Persistent) AmmoInventory.SaveInventory();
            }

            RefreshCurrentAmmoAvailable();
            _emptied = true;
        }
    }
}