using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SlashesHot : Slashes
{
    //[SerializeField] SpriteRenderer spriteRenderer;
    //[SerializeField] Collider2D col2d;

    //public float warningTime = 0.7f;
    //public bool nearMissActive = false;
    //public bool nearMissHit = false;

    private Color startColor, endColor;
    [SerializeField] GameObject hotzone;

    //scoreCounter SC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SFXManager.instance != null) SFXManager.instance.PlaySound("BladeWarning");
        StartCoroutine(BladeStrike());
        SC = FindFirstObjectByType<scoreCounter>();
        animator.speed = 1f / warningTime;
        startColor = new Color(255, 150, 0);
        endColor = Color.white;
        //startSize = 75f;
        //endSize = 3f;
        //transform.localScale = new Vector2(transform.localScale.x, startSize);
    }

    IEnumerator BladeStrike()
    {
        yield return new WaitForSeconds(warningTime);

        //spriteRenderer.enabled = false;

        animator.SetTrigger("doSlash");
        animator.speed = 1f / cleanUpTime;

        // Activate near miss
        nearMissActive = true;

        // Change color to white
        spriteRenderer.color = startColor;

        // Enable collider
        col2d.enabled = true;

        // Play slice sound
        if (SFXManager.instance != null) SFXManager.instance.PlaySound("HotKnife");

        // Deactivate near miss
        yield return new WaitForSeconds(0.1f);
        nearMissActive = false;
        
        // Change from slash to hot
        yield return new WaitForSeconds(0.3f);
        //spriteRenderer.enabled = true;
        //transform.localScale = new Vector2(transform.localScale.x, endSize);
        col2d.enabled = false;
        hotzone.transform.localScale = new Vector3(hotzone.transform.localScale.x,
            Random.Range(StageHotSpawner.instance.HotZoneLengthRange.x, StageHotSpawner.instance.HotZoneLengthRange.y),
            hotzone.transform.localScale.z);
        hotzone.AddComponent<Hazard>().damage = damage;
        hotzone.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.enabled = false;
        SC.TryAddScore();
        yield return new WaitForSeconds(2f);
        LeanTween.alpha(hotzone, 0.1f, 1f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() => {
            Destroy(gameObject);});
        
        //Destroy(animator.gameObject);
        

        

         // clean up
    }
}
