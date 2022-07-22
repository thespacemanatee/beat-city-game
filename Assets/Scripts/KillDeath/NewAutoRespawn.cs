using System.Collections;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NewAutoRespawn : AutoRespawn
{
    public NewHealth _newHealth;
    public IntVector3DictVariable positionMap;
    protected bool _dropped;

    protected override void Update()
    {
        if (GetComponent<Transform>().position.y < 0.0f && !_dropped)
        {
            Debug.Log("DROPPED detected by auto respawn");
            if (AutoRespawnDuration <= 0f)
            {
                // object is turned inactive to be able to reinstate it at respawn
                if (DisableGameObjectOnKill) gameObject.SetActive(false);
            }
            else
            {
                if (DisableAllComponentsOnKill)
                    foreach (var component in _otherComponents)
                        if (component != this)
                            component.enabled = false;

                if (_collider2D != null) _collider2D.enabled = false;
                if (_renderer != null) _renderer.enabled = false;
                _reviving = true;
                _dropped = true;
                _timeOfDeath = Time.time;
            }
        }

        if (_reviving)
            if (_timeOfDeath + AutoRespawnDuration < Time.time)
            {
                if (AutoRespawnAmount == 0) return;
                if (AutoRespawnAmount > 0)
                {
                    if (AutoRespawnRemainingAmount <= 0) return;
                    AutoRespawnRemainingAmount -= 1;
                }

                Revive();
                _reviving = false;
            }
    }

    public override void Revive()
    {
        if (AutoRespawnDuration <= 0f)
        {
            // object is turned inactive to be able to reinstate it at respawn
            gameObject.SetActive(true);
        }
        else
        {
            if (DisableAllComponentsOnKill)
                foreach (var component in _otherComponents)
                    component.enabled = true;

            if (_collider2D != null) _collider2D.enabled = true;
            if (_renderer != null) _renderer.enabled = true;
            InstantiateRespawnEffect();
            PlayRespawnSound();
        }

        //if (_health != null) _health.Revive();
        if (_newHealth != null) _newHealth.Damage(50, gameObject, 1.0f, 3.0f, GetComponent<Transform>().position);
        if (_aiBrain != null) _aiBrain.ResetBrain();
        if (_newHealth.CurrentHealth > 0) ChangePosition();
    }

    protected virtual void ChangePosition()
    {
        Debug.Log("Change Position Called");
        Vector3 newPosition = positionMap.GetRandomItem();
        newPosition.y += 2;
        transform.position = newPosition;
        StartCoroutine(CheckPosition());
    }

    private IEnumerator CheckPosition()
    {
        yield return new WaitForSeconds(0.5f);
        if (GetComponent<Transform>().position.y <= 0.0f)
        {
            ChangePosition();
        }
        else
        {
            _dropped = false;
            yield return null;
        }
    }

    protected override void InstantiateRespawnEffect()
    {
        Debug.Log("New Auto Respawn effect");
        // instantiates the destroy effect
        if (RespawnEffect != null)
        {
            var instantiatedEffect = Instantiate(RespawnEffect, transform.position, transform.rotation);
            instantiatedEffect.transform.localScale = transform.localScale;
        }
    }
}