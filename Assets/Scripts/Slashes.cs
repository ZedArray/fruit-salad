using System.Collections;
using UnityEngine;

public class Slashes : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioClip sliceSound;
    [SerializeField] AudioClip warningSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Collider2D col2d;
    
    public float warningTime = 1.5f;
    public bool nearMissActive = false;
    public bool nearMissHit = false;

    scoreCounter SC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.PlayOneShot(warningSound);
        StartCoroutine(BladeStrike());
        SC = FindFirstObjectByType<scoreCounter>(); 
    }

    public bool canNearMiss()
    {
        return nearMissActive;
    }

    IEnumerator BladeStrike()
    {
        yield return new WaitForSeconds(warningTime);

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
        Destroy(gameObject, 0.5f); // clean up
    }
}
