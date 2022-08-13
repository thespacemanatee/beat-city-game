using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    float forceAmount = 5f;
    float impactDuration = 0.2f;
    bool isHit = false;
    bool updateRotationSpeed = false;
    float rotationSpeed = 90f;
    float rotationSpeedLowerBound = 45f;
    float rotationSpeedUpperBound = 90f;
    float speedChangePercentage = 0.5f;
    float directionChangePercentage = 0.25f;
    float speedChangeLowerBound = 5f;
    float speedChangeUpperBound = 20f;
    float updateRotationSpeedInterval = 5f;

    void Start()
    {
        StartCoroutine(ToggleUpdateRotationSpeed());
    }

    void Update()
    {
        // Spin the object around the target at rotationSpeed degrees/second.
        transform.RotateAround(this.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    IEnumerator ToggleUpdateRotationSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateRotationSpeedInterval);
            if (Random.value < speedChangePercentage)
            {
                float changeValue = Random.Range(speedChangeLowerBound, speedChangeUpperBound);
                bool clockwise = rotationSpeed < 0;
                bool changeDirection = Random.value < directionChangePercentage;
                bool newClockwise = (clockwise && !changeDirection) || (!clockwise && changeDirection);
                if (Mathf.Abs(rotationSpeed) >= rotationSpeedUpperBound)
                {

                    rotationSpeed = Mathf.Max(Mathf.Abs(rotationSpeed) - changeValue, rotationSpeedLowerBound) * (newClockwise ? 1 : -1);
                }
                else
                {
                    rotationSpeed = Mathf.Min(Mathf.Abs(rotationSpeed) + changeValue, rotationSpeedUpperBound) * (newClockwise ? 1 : -1);
                }
            }
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Player") && !isHit)
        {
            Rigidbody rigidbody = c.gameObject.GetComponent<Rigidbody>();
            isHit = true;
            StartCoroutine(FakeAddForceMotion(rigidbody, c.GetContact(0).normal.normalized));
        }
    }

    IEnumerator FakeAddForceMotion(Rigidbody rigidbody, Vector3 contact)
    {
        Transform transform = rigidbody.GetComponent<Transform>();
        for (float i = 0; i < impactDuration; i += Time.deltaTime)
        {
            transform.Translate(new Vector3(-contact.x * forceAmount / impactDuration, 0, -contact.z * forceAmount / impactDuration) * Time.deltaTime);
            yield return null;
        }

        isHit = false;
        yield return null;
    }
}
