using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float despawnTime = 20.0f;
    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= despawnTime)
        {
            Destroy(gameObject);
        }
    }
}
