using System.Collections;
using MoreMountains.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public MMSimpleObjectPooler pooler;

    private void Start()
    {
        // spawn two energy packs
        for (var j = 0; j < 20; j++)
            SpawnFromPooler();
        StartCoroutine(SpawnLooper());
    }

    void SpawnFromPooler()
    {
        // static method access
        var item = pooler.GetPooledGameObject();
        if (item != null)
        {
            item.transform.position = new Vector3(Random.Range(-10f, 10f), 10, Random.Range(-10f, 10f));
            item.SetActive(true);
            
            // TODO: Clean this up
            for (var i = 0; i < item.transform.childCount; i++)
            {
                item.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("not enough items in the pool.");
        }
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