using System.Collections;
using UnityEngine;

public class Slashes : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public AudioClip sliceSound;
    [SerializeField] public AudioClip warningSound;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public Collider2D col2d;
    [SerializeField] private Animator animator;
    public float warningTime = 0.7f;
    public float cleanUpTime = 0.5f;
    public bool nearMissActive = false;
    public bool nearMissHit = false;

    public scoreCounter SC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.PlayOneShot(warningSound);
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
        if (sliceSound != null)
            audioSource.PlayOneShot(sliceSound);

        // Deactivate near miss
        yield return new WaitForSeconds(0.1f);
        nearMissActive = false;

        SC.TryAddScore();
        Destroy(gameObject, cleanUpTime); // clean up
    }
}
