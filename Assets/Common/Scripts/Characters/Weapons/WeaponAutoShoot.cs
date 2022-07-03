﻿using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     Adds this component on a weapon with a WeaponAutoAim (2D or 3D) and it will automatically shoot at targets after an
    ///     optional delay
    ///     To prevent/stop auto shoot, simply disable this component, and enable it again to resume auto shoot
    /// </summary>
    public class WeaponAutoShoot : MonoBehaviour
    {
        [Header("Auto Shoot")]
        /// the delay (in seconds) between acquiring a target and starting shooting at it
        [Tooltip("the delay (in seconds) between acquiring a target and starting shooting at it")]
        public float DelayBeforeShootAfterAcquiringTarget = 0.1f;

        protected bool _hasWeaponAndAutoAim;
        protected Transform _lastTarget;
        protected float _targetAcquiredAt;
        protected Weapon _weapon;

        protected WeaponAutoAim _weaponAutoAim;

        /// <summary>
        ///     On Awake we initialize our component
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        ///     On Update we handle auto shoot
        /// </summary>
        protected virtual void Update()
        {
            HandleAutoShoot();
        }

        /// <summary>
        ///     Grabs auto aim and weapon
        /// </summary>
        protected virtual void Initialization()
        {
            _weaponAutoAim = gameObject.GetComponent<WeaponAutoAim>();
            _weapon = gameObject.GetComponent<Weapon>();
            if (_weaponAutoAim == null)
            {
                Debug.LogWarning(name +
                                 " : the WeaponAutoShoot on this object requires that you add either a WeaponAutoAim2D or WeaponAutoAim3D component to your weapon.");
                return;
            }

            _hasWeaponAndAutoAim = _weapon != null && _weaponAutoAim != null;
        }

        /// <summary>
        ///     Checks if we have a target for enough time, and shoots if needed
        /// </summary>
        protected virtual void HandleAutoShoot()
        {
            if (!_hasWeaponAndAutoAim) return;

            if (_weaponAutoAim.Target != null)
            {
                if (_lastTarget != _weaponAutoAim.Target) _targetAcquiredAt = Time.time;

                if (Time.time - _targetAcquiredAt >= DelayBeforeShootAfterAcquiringTarget) _weapon.WeaponInputStart();
                _lastTarget = _weaponAutoAim.Target;
            }
        }
    }
}