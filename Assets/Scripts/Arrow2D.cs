using UnityEngine;

public class Arrow2D : MonoBehaviour
{
    Vector2 direction;
    public float speed = 5f;
    private bool init = false;
    private void Start()
    {
       Transform target = Fruit.instance.transform;
       direction = (target.position - transform.position).normalized;
    }
    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }


    private void OnBecameVisible()
    {
        init = true;   
    }
    private void OnBecameInvisible()
    {

        if ((init))
        {
            Destroy(gameObject);
        }
        
    }
}
