using System.Collections;
using UnityEngine;

public class Slashes : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Collider2D col2d;
    [SerializeField] public Animator animator;
    public float warningTime = 0.7f;
    public float cleanUpTime = 0.5f;
    public bool nearMissActive = false;
    public bool nearMissHit = false;

    public scoreCounter SC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SFXManager.instance != null) SFXManager.instance.PlaySound("BladeWarning");
        StartCoroutine(BladeStrike());
        animator.speed = 1f/warningTime;
        SC = FindFirstObjectByType<scoreCounter>(); 
    }

    public bool canNearMiss()
    {
        return nearMissActive;
    }

    IEnumerator BladeStrike()
    {
        yield return new WaitForSeconds(warningTime);
        animator.SetTrigger("doSlash");
        animator.speed = 1f/cleanUpTime;
        
        // Activate near miss
        nearMissActive = true;

        // Change color to white
        spriteRenderer.color = Color.white;

        // Enable collider
        col2d.enabled = true;

        // Play slice sound
        if (SFXManager.instance != null) SFXManager.instance.PlaySound("KnifeSlash");

        // Deactivate near miss
        yield return new WaitForSeconds(0.1f);
        nearMissActive = false;

        SC.TryAddScore();
        Destroy(gameObject, cleanUpTime); // clean up
    }
}
