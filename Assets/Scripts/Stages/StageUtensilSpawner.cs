using System;
using UnityEngine;
using CompositeCurves;
using Random = UnityEngine.Random;

public class StageUtensilSpawner : StageComponent<StageUtensilSpawner>

{
    [SerializeField] private CompositeCurveDefinition BalancingCurve;
    [SerializeField] private GameObject[] Utensils;
    public float spawnInterval = 4f;
    private float timer;
    private Camera mainCamera;
    private float startTime, currentTime;

    new void Awake()
    {
        base.Awake();
        startTime = Time.timeSinceLevelLoad;
    }

    private void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Fruit.dead || !doStage)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer > spawnInterval)
        {
            SpawnObject();
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
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(1, Random.value, 0));
            spawnPos.x += buffer;
        }
        else if (side == 2)
        {
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, 1, 0));
            spawnPos.y += buffer;
        }
        else if (side == 3)
        {
            spawnPos = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, 0, 0));
            spawnPos.y -= buffer;
        }

        spawnPos.z = 0;

        GameObject objectToSpawn = Utensils[Random.Range(0, Utensils.Length)];
        Instantiate(objectToSpawn, spawnPos, Quaternion.identity).GetComponent<Arrow2D>().UpdateTarget(Fruit.instance.transform);

        currentTime = Time.timeSinceLevelLoad;
        spawnInterval = BalancingCurve.Evaluate(currentTime - (startTime/2));
        print(spawnInterval);
    }
}
