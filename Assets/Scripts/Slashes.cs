using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Slashes : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioClip sliceSound;
    [SerializeField] AudioClip warningSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Collider2D col2d;
    
    public float warningTime = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.PlayOneShot(warningSound);
        StartCoroutine(BladeStrike());
    }

    IEnumerator BladeStrike()
    {
        yield return new WaitForSeconds(warningTime);

        // Change color to white
        spriteRenderer.color = Color.white;

        col2d.enabled = true;

        // Play slice sound
        if (sliceSound != null)
            audioSource.PlayOneShot(sliceSound);

        Destroy(gameObject, 0.5f); // clean up
    }
}
