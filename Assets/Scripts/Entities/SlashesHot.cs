using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SlashesHot : Slashes
{
    //[SerializeField] SpriteRenderer spriteRenderer;
    //[SerializeField] AudioClip sliceSound;
    //[SerializeField] AudioClip warningSound;
    //[SerializeField] AudioSource audioSource;
    //[SerializeField] Collider2D col2d;

    //public float warningTime = 0.7f;
    //public bool nearMissActive = false;
    //public bool nearMissHit = false;

    private Color startColor, endColor;
    private float startSize, endSize;

    //scoreCounter SC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.PlayOneShot(warningSound);
        StartCoroutine(BladeStrike());
        SC = FindFirstObjectByType<scoreCounter>();
        animator.speed = 1f / warningTime;
        startColor = new Color(255, 150, 0);
        endColor = Color.white;
        startSize = 75f;
        endSize = 3f;
        transform.localScale = new Vector2(transform.localScale.x, startSize);
    }

    IEnumerator BladeStrike()
    {
        yield return new WaitForSeconds(warningTime);

        spriteRenderer.enabled = false;

        animator.SetTrigger("doSlash");
        animator.speed = 1f / cleanUpTime;

        // Activate near miss
        nearMissActive = true;

        // Change color to white
        spriteRenderer.color = startColor;

        // Enable collider
        col2d.enabled = true;

        // Play slice sound
        if (sliceSound != null)
            audioSource.PlayOneShot(sliceSound);

        // Deactivate near miss
        yield return new WaitForSeconds(0.1f);
        nearMissActive = false;
        
        // Change from slash to hot
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.enabled = true;
        transform.localScale = new Vector2(transform.localScale.x, endSize);

        yield return new WaitForSeconds(0.2f);
        Destroy(animator.gameObject);

        float elapsedTime = 0;
        float duration = 3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        SC.TryAddScore();
        Destroy(gameObject); // clean up
    }
}
