using System.Collections.Generic;
using UnityEngine;

// falls to yLevel, then sits there as a solid obstacle (Fruit's blocker pushes out of its collider) until hazards break it
public class Shaker : MonoBehaviour, IDamageable
{
    private ShakerSpawner spawner;
    private Collider2D col2d;
    private GameObject warning;
    private float yLevel;
    private float dropTime;
    private int health;
    private int maxHealth;
    private bool landed;
    private float invincibleUntil;
    [SerializeField] private Animator anim;
    [Tooltip("Seconds of invincibility after landing and after each hit.")]
    [SerializeField] private float invincibleTime = 0.2f;

    [Header("Health bar")]
    [SerializeField] private bool showHealthBar = true;
    [SerializeField] private GameObject healthBar;
    [Tooltip("Scaled on x by the health fraction, so pivot it at the bar's left edge.")]
    [SerializeField] private Transform healthFill;
    [SerializeField] private SpriteRenderer healthFillSprite;
    [Tooltip("Fill colour by health fraction (left = empty, right = full).")]
    [SerializeField] private Gradient healthColor;

    [Header("Damage numbers")]
    [Tooltip("Leave empty to disable damage numbers.")]
    [SerializeField] private DamageNumber damageNumber;
    [SerializeField] private float damageNumberScatter = 0.3f;

    public void init(ShakerSpawner spawner, float yLevel, int health)
    {
        this.spawner = spawner;
        this.yLevel = yLevel;
        this.health = maxHealth = health;
        healthBar.SetActive(showHealthBar);
        ShowHealth();
        dropTime = Time.time + spawner.warningTime;
        warning = Instantiate(spawner.warningZone, new Vector2(transform.position.x, yLevel), Quaternion.identity);
        col2d = GetComponent<Collider2D>();
        col2d.enabled = false;
        // hazards (Slashes etc.) have no rigidbody, so a static body would never get their triggers
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (landed || Time.time < dropTime) return;
        Vector2 target = new Vector2(transform.position.x, yLevel);
        transform.position = Vector2.MoveTowards(transform.position, target, spawner.fallSpeed * Time.deltaTime);
        if ((Vector2)transform.position != target) return;
        landed = true;
        invincibleUntil = Time.time + invincibleTime;
        col2d.enabled = true;
        anim.SetTrigger("Land");
        CameraShake.shake(spawner.landShake);

        // the warning zone's own collider is the hit area, so what the player sees is what hurts
        List<Collider2D> hits = new List<Collider2D>();
        warning.GetComponent<Collider2D>().Overlap(new ContactFilter2D().NoFilter(), hits);
        foreach (Collider2D hit in hits)
        {
            Fruit fruit = hit.GetComponentInParent<Fruit>();
            if (fruit == null) continue;
            fruit.TakeDamage(spawner.landDamage);
            break; // fruit has more than one collider
        }
        Destroy(warning);
    }

    void ShowHealth()
    {
        float t = Mathf.Clamp01((float)health / maxHealth);
        healthFill.localScale = new Vector3(t, 1f, 1f);
        healthFillSprite.color = healthColor.Evaluate(t);
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0||!landed||damage<=0||Time.time < invincibleUntil) return; // already dying, don't drop coins twice
        health -= damage;
        invincibleUntil = Time.time + invincibleTime;
        anim.SetTrigger("Hurt");
        if (damageNumber != null)
            Instantiate(damageNumber, (Vector2)transform.position + Random.insideUnitCircle * damageNumberScatter, Quaternion.identity).init(damage);
        ShowHealth();
        if (health > 0) return;
        healthBar.SetActive(false);
        anim.SetBool("Dead",true);
        for (int i = 0; i < spawner.coinsDropped; i++)
        {
            Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * spawner.coinScatter;
            Instantiate(spawner.coin, spawner.ClampToBounds(pos), Quaternion.identity);
        }
        spawner.currShakers--;
        col2d.enabled = false; // stop blocking the fruit while the hurt animation plays out
        Destroy(gameObject, spawner.despawnDelay);
    }
}
