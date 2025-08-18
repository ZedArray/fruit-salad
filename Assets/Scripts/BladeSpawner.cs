using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BladeSpawner : MonoBehaviour
{
    public GameObject bladePrefab;      // assign prefab in inspector
    public float spawnInterval = 2f;    // time between blade spawns
    public float warningTime = 1.5f;    // time before slice happens
    public AudioClip sliceSound;        // assign sound in inspector

    private Camera cam;
    private AudioSource audioSource;

    void Start()
    {
        cam = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnBlade();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBlade()
    {
        // get random position somewhere in the middle of the screen
        float posX = Random.Range(250, Screen.width - 250);
        float posY = Random.Range(100, Screen.height - 100);
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(posX, posY));
        pos.z = 0;

        // get random rotation
        float rotRandom = Random.Range(0, 180);
        Quaternion rot = Quaternion.Euler(0, 0, rotRandom);

        // instantiate blade using random position and rotation
        GameObject blade = Instantiate(bladePrefab, pos, rot);
    }
}
