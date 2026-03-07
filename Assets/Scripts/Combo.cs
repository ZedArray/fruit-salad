using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    [SerializeField] NearMiss nearMissObj;
    [SerializeField] Slider comboSlider;
    [SerializeField] Image comboFill;
    [SerializeField] TextMeshProUGUI comboCounter;

    public bool abilityActive;

    private int combo;
    private float comboValue;

    float nearMissTimer = 0f;
    float nearMissDisable = 1.0f;

    float comboDecreaseTimer = 0f;
    float comboDecreaseDelay = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilityActive = false;
        comboValue = 0f;
        combo = 0;
    }

    // Update is called once per frame
    void Update()
    {
        comboCounterFunc();
        comboThing();
    }

    public void addCombo()
    {
        if (abilityActive)
        {
            comboValue = MathF.Min(1f, comboValue += 0.1f);
        }
        else 
        {
            comboValue = MathF.Min(1f, comboValue += 0.2f);
        }

        combo += 1;
        comboCounter.text = combo.ToString();
        comboDecreaseTimer = 0f;
    }

    public void resetCombo()
    {
        comboValue = 0f;
        combo = 0;
    }

    private void comboCounterFunc()
    {
        comboSlider.value = comboValue;
        if (comboValue >= 1f)
        {
            abilityActive = true;
            comboFill.color = Color.blue;
        }
        
        if (!abilityActive)
        {
            comboFill.color = Color.white;
        }

        if (comboValue <= 0.01f)
        {
            comboSlider.gameObject.SetActive(false);
            comboCounter.gameObject.SetActive(false);
            combo = 0;
            abilityActive = false;
        }
        else
        {
            comboSlider.gameObject.SetActive(true);
            comboCounter.gameObject.SetActive(true);
        }
    }

    private void comboThing()
    {
        if (nearMissObj.getNearMiss() && !abilityActive)
        {
            nearMissTimer += Time.deltaTime;
            if (nearMissTimer >= nearMissDisable)
            {
                nearMissTimer = 0f;
                nearMissObj.setNearMiss(false);
            }
        }
        else
        {
            comboDecreaseTimer += Time.deltaTime;
            if (comboDecreaseTimer >= comboDecreaseDelay)
            {
                comboValue = Mathf.Max(0f, comboValue - 0.005f);
                comboDecreaseTimer = 0f;
            }
        }
    }
}
