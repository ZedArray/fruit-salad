using UnityEngine;
using System.Collections;

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
        // spawn line across screen randomly horizontal or vertical
        bool horizontal = Random.value > 0.5f;

        Vector2 startPos, endPos;

        if (horizontal)
        {
            float y = Random.Range(-4f, 4f);
            startPos = new Vector2(-10f, y);
            endPos = new Vector2(10f, y);
        }
        else
        {
            float x = Random.Range(-6f, 6f);
            startPos = new Vector2(x, -6f);
            endPos = new Vector2(x, 6f);
        }

        GameObject blade = Instantiate(bladePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = blade.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);
            lr.startColor = lr.endColor = Color.red;
        }

        // Start coroutine to trigger slice after delay
        StartCoroutine(BladeStrike(blade, startPos, endPos));
    }

    IEnumerator BladeStrike(GameObject blade, Vector2 start, Vector2 end)
    {
        yield return new WaitForSeconds(warningTime);

        // Change color to white
        LineRenderer lr = blade.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.startColor = lr.endColor = Color.white;
        }

        // Play slice sound
        if (sliceSound != null)
            audioSource.PlayOneShot(sliceSound);

        // Raycast line to check if fruit is hit
        RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Fruit sliced! Game Over!");
                Destroy(hit.collider.gameObject); // kill fruit
            }
        }

        Destroy(blade, 0.5f); // clean up
    }
}
