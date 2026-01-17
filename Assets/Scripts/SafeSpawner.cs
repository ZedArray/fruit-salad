using System.Collections;
using UnityEngine;

public class SafeSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] boundaries;
    [SerializeField] private GameObject safeZone;
    [SerializeField] private scoreCounter scoreCounter;

    public int minimumScore;

    private bool isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = false;
    }

    private void Update()
    {
        if (!isActive && scoreCounter.score >= minimumScore)
        {
            StartCoroutine(spawnCoin());
            isActive = true;
        }
    }

    IEnumerator spawnCoin()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            float randX = Random.Range(boundaries[1].position.x + safeZone.transform.localScale.x / 2, boundaries[0].position.x - safeZone.transform.localScale.x / 2);
            float randY = Random.Range(boundaries[0].position.y - safeZone.transform.localScale.y / 2, boundaries[1].position.y + safeZone.transform.localScale.y / 2);
            Vector3 spawnloc = new Vector3(randX, randY, 0);
            GameObject safe = Instantiate(safeZone, spawnloc, Quaternion.identity);
            yield return new WaitForSeconds(5f);
            Destroy(safe.gameObject);
        }
    }
}
