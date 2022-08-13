using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabbingTile : MonoBehaviour
{
    public Material activeMaterial;
    public Material inactiveMaterial;
    private MeshRenderer meshRenderer;
    float jabDuration = 0.1f;
    float jabLength = 3f;
    float originalLength;
    float jabExtendedDuration = 0.5f;
    float jabCooldownDuration = 5f;
    bool canJab = false;
    float jabPercentage = 0.1f;
    float delayJabDurationLowerBound = 1f;
    float delayJabDurationUpperBound = 5f;
    bool delaying = false;
    bool jabbing = false;
    float impactDuration = 0.2f;
    float collisionForceAmount = 10f;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalLength = transform.localScale.y;
        Debug.Log("Original length: " + originalLength);
        StartCoroutine(DelayJab());
    }

    // Update is called once per frame
    void Update()
    {
        if (canJab && Random.value < jabPercentage && !delaying)
        {
            Jab();
        }
        else if (canJab && !delaying)
        {
            // Only want to check if can jab every 3s instead of 60fps
            StartCoroutine(DelayJab());
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Player") && jabbing)
        {
            Rigidbody rigidbody = c.gameObject.GetComponent<Rigidbody>();
            // rigidbody.velocity = new Vector3(50f, 50f, 50f);
            // Debug.Log("Collided " + c.gameObject + ", Direction: " + c.GetContact(0).normal.normalized);
            // isHit = true;
            StartCoroutine(FakeAddForceMotion(rigidbody, c.GetContact(0).normal.normalized));
            // Debug.Log(c.gameObject.transform.forward);
        }
    }

    IEnumerator FakeAddForceMotion(Rigidbody rigidbody, Vector3 contact)
    {
        Transform transform = rigidbody.GetComponent<Transform>();
        for (float i = 0; i < impactDuration; i += Time.deltaTime)
        {
            transform.Translate(new Vector3(0, collisionForceAmount / impactDuration, 0) * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    void Jab()
    {
        StartCoroutine(AnimateJab());
    }

    IEnumerator DelayJab()
    {
        delaying = true;
        canJab = false;
        yield return new WaitForSeconds(Random.Range(delayJabDurationLowerBound, delayJabDurationUpperBound));
        canJab = true;
        delaying = false;
    }


    IEnumerator AnimateJab()
    {
        canJab = false;
        int loopCount = 0;

        // Jab animation
        jabbing = true;
        meshRenderer.material = activeMaterial;
        for (float i = 0; i < jabDuration; i += Time.deltaTime)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + ((jabLength * 2 - originalLength) * Time.deltaTime / jabDuration), transform.localScale.z);
            loopCount++;
            yield return null;
        }

        float newLength = transform.localScale.y;

        yield return new WaitForSeconds(jabExtendedDuration);

        // Back to normal animation
        for (float i = 0; i < jabDuration; i += Time.deltaTime)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - ((newLength - originalLength) * Time.deltaTime / jabDuration), transform.localScale.z);
            yield return null;
        }
        jabbing = false;
        meshRenderer.material = inactiveMaterial;

        transform.localScale = new Vector3(transform.localScale.x, originalLength, transform.localScale.z);
        // Cooldown
        yield return new WaitForSeconds(jabCooldownDuration);
        canJab = true;

        yield return null;
    }
}
