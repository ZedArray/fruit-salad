using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using CompositeCurves;
using Random = UnityEngine.Random;

public class BladeSpawner : StageComponent<BladeSpawner>
{
    [SerializeField] private GameObject _fruit;

    public GameObject bladePrefab;      // assign prefab in inspector
    public float spawnInterval = 2f;    // time between blade spawns
    public AudioClip sliceSound;        // assign sound in inspector

    private Camera cam;
    private AudioSource audioSource;
    private float maxX, maxY, minX, minY;
    private float timer;

    new void Awake()
    {
        base.Awake();
    }

    [SerializeField] private CompositeCurveDefinition balancingCurve;
    void Start()
    {
        cam = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        minX = -7.5f;
        maxX = 7.5f;
        minY = -4f;
        maxY = 4f;
        timer = 0f;
        //StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        if (Fruit.dead || doStage == false)
        {
            return;
        }

        timer += Time.deltaTime;
        
        if (timer > spawnInterval)
        {
            SpawnBlade();
            timer = 0f;
        }
    }
    void SpawnBlade()
    {
        Vector3 fruitPos = _fruit.transform.position;
        float fruitX = fruitPos.x;
        float fruitY = fruitPos.y;
        float posX = Random.Range(Mathf.Max(minX, fruitX - 3), Mathf.Min(maxX, fruitX + 3));
        float posY = Random.Range(Mathf.Max(minY, fruitY - 2), Mathf.Min(maxY, fruitY + 2));
        Vector3 pos = new Vector3(posX, posY);
        pos.z = 0;
        // get random rotation
        float rotRandom = Random.Range(0, 180);
        Quaternion rot = Quaternion.Euler(0, 0, rotRandom);
        // instantiate blade using random position and rotation
        GameObject blade = Instantiate(bladePrefab, pos, rot);
        
        spawnInterval = balancingCurve.Evaluate(Time.timeSinceLevelLoad);
        Debug.LogWarning(spawnInterval);
    }
}
