using UnityEngine;

// damages whatever IDamageable touches this object's collider; for hazards with no script of their own (e.g. the hot zone)
public class Hazard : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable target)) target.TakeDamage(damage);
    }
}
