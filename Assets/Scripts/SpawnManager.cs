using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

public class SpawnManager : MonoBehaviour, MMEventListener<EnergyDropEvent>
{
    public MMSimpleObjectPooler pooler;
    public IntVector3DictVariable positionMap;
    public int initialCount = 10;

    private void Start()
    {
        // spawn two energy packs
        for (var j = 0; j < initialCount; j++) SpawnFromPooler();
        StartCoroutine(SpawnLooper());
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(EnergyDropEvent eventType)
    {
        var count = eventType.Count;
        var position = eventType.Position;
        for (var i = 0; i < count; i++)
        {
            var item = SpawnFromPooler(position + Vector3.up * 5f);
            if (item == null) continue;
            var rigidBody = item.GetComponent<Rigidbody>();
            if (rigidBody)
            {
                // fling in random direction
                rigidBody.AddForce(Random.insideUnitSphere * 10f + Vector3.up * 5f, ForceMode.Impulse);
            }
        }
    }

    private void SpawnFromPooler()
    {
        // SpawnFromPooler(new Vector3(Random.Range(-10f, 10f), 10f, Random.Range(-10f, 10f)));
        Vector3 newPosition = positionMap.GetRandomItem();
        newPosition.y += 5;
        SpawnFromPooler(newPosition);
    }

    private GameObject SpawnFromPooler(Vector3 position)
    {
        // static method access
        var item = pooler.GetPooledGameObject();
        if (item != null)
        {
            item.transform.position = position;
            item.SetActive(true);

            // TODO: Clean this up
            item.GetComponent<BoxCollider>().enabled = true;
            foreach (Transform child in item.transform) child.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("not enough items in the pool.");
        }

        return item;
    }

    private IEnumerator SpawnLooper()
    {
        while (true)
        {
            SpawnFromPooler();
            yield return new WaitForSeconds(1f);
        }
    }
}