using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class NewHitscanWeapon : HitscanWeapon
{
    public float thickness = 3f; //<-- Desired thickness here.

    private RaycastHit[] _hits;
    private GameObject[] _hitObjects;
    private Vector3[] _hitPoints;
    private LineRenderer _line;
    private bool _isShooting;
    private bool _isReleased;

    protected override void Update()
    {
        if (_isShooting)
        {
            DetermineSpawnPosition();
            DetermineDirection();
            SpawnProjectile(SpawnPosition, true);
            HandleDamage();
        }
    }

    public override void WeaponUse()
    {
        if (RecoilForce > 0f && _controller != null)
        {
            if (Owner != null)
            {
                if (!_controllerIs3D)
                {
                    if (Flipped)
                    {
                        _controller.Impact(transform.right, RecoilForce);
                    }
                    else
                    {
                        _controller.Impact(-transform.right, RecoilForce);
                    }
                }
                else
                {
                    _controller.Impact(-transform.forward, RecoilForce);
                }
            }
        }

        TriggerWeaponUsedFeedback();
        if (_isShooting || _isReleased) return;
        _isShooting = true;
        DetermineSpawnPosition();
        DetermineDirection();
        _line = gameObject.GetComponentInChildren(typeof(LineRenderer)) as LineRenderer;
        if (_line != null)
        {
            _line.enabled = true;
        }

        SpawnProjectile(SpawnPosition, true);
        HandleDamage();
        StartCoroutine(StartShootingDuration());
    }

    public override void SpawnProjectile(Vector3 spawnPosition, bool triggerObjectActivation = true)
    {
        _hitObject = null;
        // we cast a ray in the direction
        if (Mode == Modes.ThreeD)
        {
            // if 3D
            _origin = SpawnPosition;

            _hits = Physics.SphereCastAll(_origin, thickness, _randomSpreadDirection, HitscanMaxDistance);
            // if we've hit something, our destination is the raycast hit
            if (_hits != null)
            {
                _hitObjects = new GameObject[_hits.Length];
                _hitPoints = new Vector3[_hits.Length];
                for (var i = 0; i < _hits.Length; i++)
                {
                    try
                    {
                        var hit = _hits[i];
                        if (hit.collider.gameObject.name.Contains("MinimalCharacter") &&
                            hit.collider.gameObject.name != this.Owner.name)
                        {
                            _hitObject = hit.collider.gameObject;
                            _hitObjects[i] = _hitObject;
                            _hitPoint = _hit.point;
                            _hitPoints[i] = _hitPoint;
                        }
                    }
                    catch
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
            _hit2D = MMDebug.RayCast(_origin, _randomSpreadDirection, HitscanMaxDistance, HitscanTargetLayers,
                Color.red, true);
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

    private IEnumerator StartShootingDuration()
    {
        //yield on a new YieldInstruction that waits for 3 seconds.
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Destroying laser");
        _isShooting = false;
        _isReleased = true;
        _line.enabled = false;
        WeaponState.ChangeState(WeaponStates.WeaponStop);
        Debug.Log("Changed to weapon stop state");
    }


    protected override void HandleDamage()
    {
        if (_hitObjects.Length == 0)
        {
            return;
        }

        foreach (var hit in _hitObjects)
        {
            if (hit == null)
            {
                continue;
            }

            _health = hit.MMGetComponentNoAlloc<Health>();
            if (_health == null)
            {
                // hit non damageable
                if (HitNonDamageable != null)
                {
                    HitNonDamageable.transform.position = _hitPoint;
                    HitNonDamageable.transform.LookAt(transform);
                    HitNonDamageable.PlayFeedbacks();
                }

                if (NonDamageableImpactParticles != null)
                {
                    NonDamageableImpactParticles.transform.position = _hitPoint;
                    NonDamageableImpactParticles.transform.LookAt(transform);
                    NonDamageableImpactParticles.Play();
                }
            }
            else
            {
                // hit damageable
                _damageDirection = (hit.transform.position - transform.position).normalized;

                var randomDamage = Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));
                _health.Damage(randomDamage, gameObject, DamageCausedInvincibilityDuration,
                    DamageCausedInvincibilityDuration, _damageDirection, TypedDamages);

                if (HitDamageable != null)
                {
                    HitDamageable.transform.position = _hitPoint;
                    HitDamageable.transform.LookAt(this.transform);
                    HitDamageable.PlayFeedbacks();
                }

                if (DamageableImpactParticles != null)
                {
                    DamageableImpactParticles.transform.position = _hitPoint;
                    DamageableImpactParticles.transform.LookAt(transform);
                    DamageableImpactParticles.Play();
                }
            }
        }
    }
}