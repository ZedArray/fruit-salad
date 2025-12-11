using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NearMiss : MonoBehaviour
{
    [SerializeField] SpriteRenderer circle;
    [SerializeField] Slider comboSlider;
    [SerializeField] Image comboFill;
    [SerializeField] TextMeshProUGUI comboCounter;

    private int combo;
    private bool nearMiss;
    private float comboValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(fadeOut());
        StartCoroutine(comboThing());
        comboValue = 0f;
        combo = 0;
        nearMiss = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Fruit.instance.transform.position;
        comboSlider.value = comboValue;
        if (comboValue >= 0.99f)
        {
            comboFill.color = Color.blue;
        }
        else
        {
            comboFill.color = Color.white;
        }

        if (comboValue <= 0.01f)
        {
            comboSlider.gameObject.SetActive(false);
            comboCounter.gameObject.SetActive(false);
            combo = 0;
        }
        else
        {
            comboSlider.gameObject.SetActive(true);
            comboCounter.gameObject.SetActive(true);
        }
    }

    IEnumerator comboThing()
    {
        while (true)
        {
            if (nearMiss)
            {
                yield return new WaitForSeconds(1f);
                nearMiss = false;
            }
            yield return new WaitForSeconds(0.05f);
            comboValue = Mathf.Max(0f, comboValue - 0.005f);
        }
        
    }

    IEnumerator fadeOut()
    {
        while (true)
        {
            Color newColor = circle.color;
            newColor.a -= 0.01f;
            circle.color = newColor;
            yield return new WaitForSeconds(0.005f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") && !Fruit.dead)
        {
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0.8f);
            comboValue = MathF.Min(1f, comboValue += 0.2f);
            nearMiss = true;
            combo += 1;
            comboCounter.text = combo.ToString();
        }
    }
}
