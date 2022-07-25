using System.Collections;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System;

public class RainFireWeapon : ProjectileWeapon
{
    public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles,
        bool triggerObjectActivation = true)
    {
        /// we get the next object in the pool and make sure it's not null
        var nextGameObject = ObjectPooler.GetPooledGameObject();

        // mandatory checks
        if (nextGameObject == null)
        {
            return null;
        }

        if (nextGameObject.GetComponent<MMPoolableObject>() == null)
        {
            throw new Exception(gameObject.name +
                                " is trying to spawn objects that don't have a PoolableObject component.");
        }

        StartCoroutine(RotateProjectile(nextGameObject, spawnPosition));
        if (triggerObjectActivation && nextGameObject.GetComponent<MMPoolableObject>() != null)
        {
            nextGameObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
        }

        return nextGameObject;
    }

    private IEnumerator RotateProjectile(GameObject nextGameObject, Vector3 spawnPosition)
    {
        for (var i = 0; i < 8; i++)
        {
            float tiltAroundY = i * 45;
            var spawnedProjectile = Instantiate(nextGameObject);
            //Position
            spawnedProjectile.transform.position = spawnPosition + new Vector3(0, 5, 0);
            spawnedProjectile.transform.Rotate(0, tiltAroundY, 0);
            if (_projectileSpawnTransform != null)
            {
                spawnedProjectile.transform.position = _projectileSpawnTransform.position;
            }

            var projectile = spawnedProjectile.GetComponent<Projectile>();
            if (projectile == null) continue;
            projectile.SetWeapon(this);
            spawnedProjectile.gameObject.SetActive(true);
            var newRotationAngle = Quaternion.Euler(0, tiltAroundY, 0);
            if (Owner == null)
            {
                projectile.SetDirection(newRotationAngle * transform.rotation * DefaultProjectileDirection,
                    transform.rotation, true);
            }
            else
            {
                projectile.SetOwner(Owner.gameObject);
                if (Owner.CharacterDimension == Character.CharacterDimensions.Type3D)
                {
                    projectile.SetDirection(newRotationAngle * transform.forward, transform.rotation, true);
                }
                else
                {
                    var newDirection = (newRotationAngle * transform.right) * (Flipped ? -1 : 1);
                    projectile.SetDirection(newDirection, transform.rotation,
                        Owner.Orientation2D == null || Owner.Orientation2D.IsFacingRight);
                }
            }

            if (RotateWeaponOnSpread)
            {
                transform.rotation *= newRotationAngle;
            }
        }

        yield return null;
    }
}