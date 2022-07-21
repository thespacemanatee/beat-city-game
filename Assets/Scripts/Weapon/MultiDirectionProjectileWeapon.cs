using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;

public class MultiDirectionProjectileWeapon : ProjectileWeapon
{

 public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
 {
    /// we get the next object in the pool and make sure it's not null
    GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

    // mandatory checks
    if (nextGameObject == null) { return null; }
    if (nextGameObject.GetComponent<MMPoolableObject>() == null)
    {
        throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
    }

    for (int i = 0; i< 8; i++)
    {
        GameObject spawnedProjectile = Instantiate(nextGameObject);
        spawnedProjectile.transform.position = spawnPosition;
        if (_projectileSpawnTransform != null)
        {
            spawnedProjectile.transform.position = _projectileSpawnTransform.position;
        }
        Projectile projectile = spawnedProjectile.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetWeapon(this);
            if (Owner != null)
            {
                projectile.SetOwner(Owner.gameObject);
            }
        }
        spawnedProjectile.gameObject.SetActive(true);
        if (projectile != null)
        {
            float tiltAroundX = Input.GetAxis("Vertical") * i * 45;
            Quaternion newDirectionAngle = Quaternion.Euler(tiltAroundX, 0, 0);
            if (Owner == null)
            {
                projectile.SetDirection(newDirectionAngle * transform.rotation * DefaultProjectileDirection, transform.rotation, true);
            }
            else
            {
                if (Owner.CharacterDimension == Character.CharacterDimensions.Type3D)
                {
                    projectile.SetDirection(newDirectionAngle * transform.forward, transform.rotation, true);
                }
                else
                {
                    Vector3 newDirection = (newDirectionAngle * transform.right) * (Flipped ? -1 : 1);
                    if (Owner.Orientation2D != null)
                    {
                        projectile.SetDirection(newDirection, transform.rotation, Owner.Orientation2D.IsFacingRight);
                    }
                    else
                    {
                        projectile.SetDirection(newDirection, transform.rotation, true);
                    }
                }
            }
            if (RotateWeaponOnSpread)
            {
                this.transform.rotation = this.transform.rotation * newDirectionAngle;
            }
        }  
    }
    if (triggerObjectActivation)
    {
        if (nextGameObject.GetComponent<MMPoolableObject>() != null)
        {
            nextGameObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
        }
    }
    return (nextGameObject);
 }
}