using System;
using UnityEngine;

public class Arrow2D : MonoBehaviour
{
    Vector2 direction;
    public float speed = 5f;
    private bool init = false;
    private static scoreCounter SC;
    private bool GiveScore = false;
    private int damage;
    private void Start()
    {
        if (SC == null)
        {
            SC = FindFirstObjectByType<scoreCounter>(); 
        }
    }

    public void UpdateTarget(Transform target, int damage, bool GiveScore = false)
    {
        direction = (target.position - transform.position).normalized;
        this.GiveScore = GiveScore;
        this.damage = damage;
    }
    
    public void UpdateTarget(Vector3 target, int damage, bool GiveScore = false)
    {
        direction = (target - transform.position).normalized;
        this.GiveScore = GiveScore;
        this.damage = damage;
    }

    void Update()
    {
        transform.position += (Vector3)direction * (speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable target)) target.TakeDamage(damage);
    }

    public bool canNearMiss()
    {
        return true;
    }

    private void OnBecameVisible()
    {
        init = true;   
    }
    private void OnBecameInvisible()
    {

        if ((init))
        {
            if(GiveScore)
            {
                SC.TryAddScore();
            }
            Destroy(gameObject);
        }
        
    }
}
