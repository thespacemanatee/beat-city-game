using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    public class CharacterDisableIKOnReload : MonoBehaviour
    {
        [Header("IK")] public WeaponIK BoundWeaponIK;

        public bool DetachLeftHand = true;
        public bool DetachRightHand;

        [Header("Weapon Models")] public bool DisableAimWeaponModelAtTargetDuringReload = true;

        public List<WeaponModel> WeaponModels;

        protected CharacterHandleWeapon _handleWeapon;
        protected float _reloadDuration;

        protected virtual void Start()
        {
            _handleWeapon = gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
        }

        protected virtual void LateUpdate()
        {
            if (_handleWeapon == null || !_handleWeapon.isActiveAndEnabled) return;
            if (_handleWeapon.CurrentWeapon == null) return;

            if (_handleWeapon.CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart)
            {
                _reloadDuration = _handleWeapon.CurrentWeapon.ReloadTime;
                StartCoroutine(ReloadSequence());
            }
        }

        public void ForceDisable()
        {
            StartCoroutine(ReloadSequence());
        }

        protected virtual IEnumerator ReloadSequence()
        {
            if (DetachLeftHand) BoundWeaponIK.AttachLeftHand = false;
            if (DetachRightHand) BoundWeaponIK.AttachRightHand = false;
            if (DisableAimWeaponModelAtTargetDuringReload)
                foreach (var model in WeaponModels)
                    model.AimWeaponModelAtTarget = false;
            yield return MMCoroutine.WaitFor(_reloadDuration);
            if (DetachLeftHand) BoundWeaponIK.AttachLeftHand = true;
            if (DetachRightHand) BoundWeaponIK.AttachRightHand = true;
            if (DisableAimWeaponModelAtTargetDuringReload)
                foreach (var model in WeaponModels)
                    model.AimWeaponModelAtTarget = true;
        }
    }
}