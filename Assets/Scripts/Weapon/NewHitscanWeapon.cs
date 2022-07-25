using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = System.Random;

public class NewHitscanWeapon : HitscanWeapon
{
    RaycastHit[] hits;
    GameObject[] hitObjects;
    Vector3[]  hitPoints;
    LineRenderer line;
    private bool shooting;
    private bool released = false;
    private float thickness = 3f; //<-- Desired thickness here.

    protected override void Update()
    {
        if (shooting)
        {
            DetermineSpawnPosition();
            DetermineDirection();
            SpawnProjectile(SpawnPosition, true);
            HandleDamage();
        }
    }

    public override void WeaponUse()
    {
        if ((RecoilForce > 0f) && (_controller != null))
        {
            if (Owner != null)
            {
                if (!_controllerIs3D)
                {
                    if (Flipped)
                    {
                        _controller.Impact(this.transform.right, RecoilForce);
                    }
                    else
                    {
                        _controller.Impact(-this.transform.right, RecoilForce);
                    }
                }
                else
                {
                    _controller.Impact(-this.transform.forward, RecoilForce);
                }
            }
        }

        TriggerWeaponUsedFeedback();
        if (!shooting && !released)
        {
            shooting = true;
            DetermineSpawnPosition();
            DetermineDirection();
            line = gameObject.GetComponentInChildren(typeof(LineRenderer)) as LineRenderer;
            if (line != null)
            {
                line.enabled = true;
            }
            SpawnProjectile(SpawnPosition, true);
            HandleDamage();
            StartCoroutine(startShootingDuration());
        }
    }

    public override void SpawnProjectile(Vector3 spawnPosition, bool triggerObjectActivation = true)
    {
        _hitObject = null;
        // we cast a ray in the direction
        if (Mode == Modes.ThreeD)
        {
            // if 3D
            _origin = SpawnPosition;
            
            hits = Physics.SphereCastAll(_origin, thickness, _randomSpreadDirection, HitscanMaxDistance);
            // if we've hit something, our destination is the raycast hit
            if (hits != null)
            {   
                hitObjects = new GameObject[hits.Length];
                hitPoints = new Vector3[hits.Length];
                for (int i = 0; i < hits.Length; i++)
                {
                    try
                    {
                        RaycastHit hit = hits[i];
                        if (hit.collider.gameObject.name.Contains("MinimalCharacter") && hit.collider.gameObject.name != this.Owner.name)
                        {
                            _hitObject = hit.collider.gameObject;
                            hitObjects[i] = _hitObject;
                            _hitPoint = _hit.point;
                            hitPoints[i] = _hitPoint;
                        }
                    } catch
                    {
                        //Collision with wrong object.
                    }
                    
                }
            }
            // otherwise we just draw our laser in front of our weapon 
            else
            {
                _hitObject = null;
            }
        }
        else
        {
            // if 2D

            //_direction = this.Flipped ? Vector3.left : Vector3.right;

            // we cast a ray in front of the weapon to detect an obstacle
            _origin = SpawnPosition;
            _hit2D = MMDebug.RayCast(_origin, _randomSpreadDirection, HitscanMaxDistance, HitscanTargetLayers, Color.red, true);
            if (_hit2D)
            {
                _hitObject = _hit2D.collider.gameObject;
                _hitPoint = _hit2D.point;
            }
            // otherwise we just draw our laser in front of our weapon 
            else
            {
                _hitObject = null;
            }
        }      
    }

    IEnumerator startShootingDuration()
    {
        //yield on a new YieldInstruction that waits for 3 seconds.
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Destroying laser");
        this.shooting = false;
        this.released = true;   
        this.line.enabled = false;
        //Destroy(this.gameObject); 
        WeaponState.ChangeState(WeaponStates.WeaponIdle);
        Debug.Log("Changed to idle state");
    }


    protected override void HandleDamage()
    {
        if (hitObjects.Length == 0)
        {
            return;
        }
        for (int i = 0; i < hitObjects.Length; i++)
        {
            if (hitObjects[i] == null)
            {
                continue;
            }
            _health = hitObjects[i].MMGetComponentNoAlloc<Health>();
            if (_health == null)
			{
				// hit non damageable
				if (HitNonDamageable != null)
				{
					HitNonDamageable.transform.position = _hitPoint;
					HitNonDamageable.transform.LookAt(this.transform);
					HitNonDamageable.PlayFeedbacks();
				}

				if (NonDamageableImpactParticles != null)
				{
					NonDamageableImpactParticles.transform.position = _hitPoint;
					NonDamageableImpactParticles.transform.LookAt(this.transform);
					NonDamageableImpactParticles.Play();
				}
			}
			else
			{
				// hit damageable
				_damageDirection = (hitObjects[i].transform.position - this.transform.position).normalized;
                
				float randomDamage = UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));
				_health.Damage(randomDamage, this.gameObject, DamageCausedInvincibilityDuration, DamageCausedInvincibilityDuration, _damageDirection, TypedDamages);

				if (HitDamageable != null)
				{
					HitDamageable.transform.position = _hitPoint;
					HitDamageable.transform.LookAt(this.transform);
					HitDamageable.PlayFeedbacks();
				}
                
				if (DamageableImpactParticles != null)
				{
					DamageableImpactParticles.transform.position = _hitPoint;
					DamageableImpactParticles.transform.LookAt(this.transform);
					DamageableImpactParticles.Play();
				}
			}
        }

    }
}
