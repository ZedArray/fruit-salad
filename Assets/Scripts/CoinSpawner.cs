using System.Collections;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] boundaries;
    [SerializeField] private GameObject coin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnCoin());
    }

    IEnumerator spawnCoin()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            float randX = Random.Range(boundaries[1].position.x, boundaries[0].position.x);
            float randY = Random.Range(boundaries[0].position.y, boundaries[1].position.y);
            Vector3 spawnloc = new Vector3(randX, randY, 0);
            Instantiate(coin, spawnloc, Quaternion.identity);
        }
    }
}
