using System.Collections;
using UnityEngine;

public class SafeSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] boundaries;
    [SerializeField] private GameObject safeZone;
    [SerializeField] private scoreCounter scoreCounter;

    private bool isActive;
    private bool megaDeathActive;

    public int minimumScore;
    public float intervalTime;
    public float beforeDeathTime;
    public float afterDeathTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = false;
        megaDeathActive = false;
    }

    private void Update()
    {
        if (!isActive && scoreCounter.score >= minimumScore)
        {
            StartCoroutine(spawnSafe());
            isActive = true;
        }
        if (megaDeathActive)
        {
            Fruit.instance.kill();
        }
    }

    IEnumerator spawnSafe()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalTime);
            float randX = Random.Range(boundaries[1].position.x + safeZone.transform.localScale.x / 2, boundaries[0].position.x - safeZone.transform.localScale.x / 2);
            float randY = Random.Range(boundaries[0].position.y - safeZone.transform.localScale.y / 2, boundaries[1].position.y + safeZone.transform.localScale.y / 2);
            Vector3 spawnloc = new Vector3(randX, randY, 0);
            GameObject safe = Instantiate(safeZone, spawnloc, Quaternion.identity);
            yield return new WaitForSeconds(beforeDeathTime);
            megaDeathActive = true;
            yield return new WaitForSeconds(afterDeathTime);
            megaDeathActive = false;
            Destroy(safe.gameObject);
        }
    }
}
