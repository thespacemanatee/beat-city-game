using System.Collections;
using System.Numerics;
using MoreMountains.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SpawnManager : MonoBehaviour, MMEventListener<EnergyDropEvent>
{
    public MMSimpleObjectPooler pooler;
    public IntVector3DictVariable positionMap;
    public int initialCount = 10;
    public float spawnInterval = 1f;

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
            var spawnPosition = position + new Vector3(0, 5, 0);
            var item = SpawnFromPooler(spawnPosition);
            if (item == null) item = Instantiate(pooler.GameObjectToPool, spawnPosition, Quaternion.identity);
            var rigidBody = item.GetComponent<Rigidbody>();
            if (!rigidBody) continue;
            // fling in random direction
            var direction = Random.insideUnitSphere;
            rigidBody.AddForce(direction * 10 + Vector3.up * 15f, ForceMode.Impulse);
        }
    }

    private void SpawnFromPooler()
    {
        var newPosition = positionMap.GetRandomItem();
        newPosition.y += 10;
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
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}