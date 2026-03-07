using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NearMiss : MonoBehaviour
{
    [SerializeField] SpriteRenderer circle;

    [SerializeField] Combo combo;
    [SerializeField] Slider comboSlider;
    [SerializeField] Image comboFill;
    [SerializeField] TextMeshProUGUI comboCounter;
    [SerializeField] Collider2D col;

    private bool nearMiss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(fadeOut());
        nearMiss = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Fruit.instance.transform.position;
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
        print(collision.gameObject.name);
        if (collision.CompareTag("Slash"))
        {
            if (collision.GetComponent<Arrow2D>())
            {
                if (collision.GetComponent<Arrow2D>().canNearMiss())
                {
                    StartCoroutine(Miss());
                }
            }
            else if (collision.GetComponent<Slashes>())
            {
                if(collision.GetComponent<Slashes>().canNearMiss())
                {
                    StartCoroutine(Miss());
                }
            }
        }
    }

    IEnumerator Miss()
    {
        yield return new WaitForEndOfFrame();
        {
            if (!Fruit.hit)
            {
                circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0.8f);
                nearMiss = true;
                combo.addCombo();
            }
        }
    }

    public bool getNearMiss()
    {
        return nearMiss;
    }

    public void setNearMiss(bool _nearMiss)
    {
        nearMiss = _nearMiss;
    }
}
