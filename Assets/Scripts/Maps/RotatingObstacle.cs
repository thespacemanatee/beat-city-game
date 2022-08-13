using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    float forceAmount = 5f;
    float impactDuration = 0.2f;
    bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // Spin the object around the target at 20 degrees/second.
        transform.RotateAround(this.transform.position, Vector3.up, 180 * Time.deltaTime);
    }


    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Player") && !isHit)
        {
            Rigidbody rigidbody = c.gameObject.GetComponent<Rigidbody>();
            // rigidbody.velocity = new Vector3(50f, 50f, 50f);
            // Debug.Log("Collided " + c.gameObject + ", Direction: " + c.GetContact(0).normal.normalized);
            isHit = true;
            StartCoroutine(FakeAddForceMotion(rigidbody, c.GetContact(0).normal.normalized));
            // Debug.Log(c.gameObject.transform.forward);
        }
    }

    IEnumerator FakeAddForceMotion(Rigidbody rigidbody, Vector3 contact)
    {
        Transform transform = rigidbody.GetComponent<Transform>();
        // float i = 0.01f;
        for (float i = 0; i < impactDuration; i += Time.deltaTime)
        {
            transform.Translate(new Vector3(-contact.x * forceAmount / impactDuration, 0, -contact.z * forceAmount / impactDuration) * Time.deltaTime);
            yield return null;
        }

        // while (forceAmount > i)
        // {

        //     // rigidbody.velocity = new Vector3(contact.x * forceAmount / i, contact.y * forceAmount / i, contact.z * forceAmount / i);
        //     i = i + Time.deltaTime;
        //     yield return new WaitForEndOfFrame();
        // }
        // rigidbody.velocity = Vector2.zero;
        isHit = false;
        yield return null;
    }
}
