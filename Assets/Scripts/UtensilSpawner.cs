using UnityEngine;

public class UtensilSpawner : MonoBehaviour
{

    public GameObject[] Utensils;
    public float cooldown;
    private float timer;
    private Camera mainCamera;
    public float maxIdleTime;
    public static UtensilSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {

        if (Fruit.dead)
        {
            return;
        }
        if (Fruit.idle >= maxIdleTime)
        {
            if (timer <= 0)
            {
                SpawnObject();
                timer = cooldown;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        else {
            timer = 0;
        }
    }

    void SpawnObject()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        int side = Random.Range(0, 4);

        Vector3 spawnPos = Vector3.zero;
        float buffer = 3f;

        if (side == 0) 
        {
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(0, Random.value, 0));
            spawnPos.x -= buffer;
        }
        else if (side == 1) 
        {
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(1, Random.value,0));
            spawnPos.x += buffer;
        }
        else if (side == 2)
        {
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, 1,0));
            spawnPos.y += buffer;
        }
        else if (side == 3) 
        {
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, 0, 0));
            spawnPos.y -= buffer;
        }

        spawnPos.z = 0;

        GameObject objectToSpawn = Utensils[Random.Range(0, Utensils.Length)];
        Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
    }
}
