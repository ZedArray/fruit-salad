using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class StageManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private float stageNameDuration = 2f;
    [SerializeField] private float stageNameFlyTime = 0.35f;
    private Vector2 stageNameRest;
    private Coroutine stageNameRoutine;


    private StageComponentNG[] components;
    private int stage;
    private int stageAmount;

    private bool repeating;
    public string currentStage => stage >= 0 ? stageData.stages[stage].name : "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stage = -1;
        stageAmount = stageData.stages.Length;
        if (stageNameText != null)
        {
            stageNameRest = stageNameText.rectTransform.anchoredPosition;
            stageNameText.gameObject.SetActive(false);
        }
        populateComponents();
        ChangeStage();
    }

    private void populateComponents()
    {
        components = new StageComponentNG[stageData.stageManagersClassNames.Length];

        for (int i = 0; i < stageData.stageManagersClassNames.Length; i++)
        {
            Type t = Type.GetType(stageData.stageManagersClassNames[i]);
            components[i] = (StageComponentNG)FindFirstObjectByType(t);
            components[i].Disable();
        }
    }

    [ContextMenu("Test Inheritance Fuckery")]
    public void TestInheritanceFuckery()
    {
        Type t = Type.GetType(stageData.stageManagersClassNames[0]);
        StageComponentNG instance = (StageComponentNG)FindFirstObjectByType(t);
        instance.Enable();
        
    }

    private void DisableAll()
    {
        for (int i = 0; i < stageData.stageManagersClassNames.Length; i++)
        {
            components[i].Disable();
        }
    }

    // flies in from the right, holds, flies out to the left
    private IEnumerator ShowStageName(string stageName)
    {
        RectTransform rt = stageNameText.rectTransform;
        Vector2 off = Vector2.right * ((RectTransform)rt.parent).rect.width;
        stageNameText.text = stageName;
        stageNameText.gameObject.SetActive(true);
        yield return Fly(rt, stageNameRest + off, stageNameRest);
        yield return new WaitForSeconds(stageNameDuration);
        yield return Fly(rt, stageNameRest, stageNameRest - off);
        stageNameText.gameObject.SetActive(false);
    }

    private IEnumerator Fly(RectTransform rt, Vector2 from, Vector2 to)
    {
        for (float t = 0; t < stageNameFlyTime; t += Time.deltaTime)
        {
            rt.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0, 1, t / stageNameFlyTime));
            yield return null;
        }
        rt.anchoredPosition = to;
    }

    public void ChangeStage()
    {
        if (stage == stageAmount - 1)
        {
            repeating = true;
        }

        stage = (stage + 1) % stageAmount;
        while (repeating && stageData.stages[stage].skipOnRepeat)
        {
            print("Skipping stage " + stageData.stages[stage].name );
            stage = (stage + 1) % stageAmount;
        }

        StageDefinition st = stageData.stages[stage];
        if (stageNameText != null && !st.hideName)
        {
            if (stageNameRoutine != null) StopCoroutine(stageNameRoutine);
            stageNameRoutine = StartCoroutine(ShowStageName(st.name));
        }
        DisableAll();
        foreach (int i in st.enabledStages)
        {
            if (i >= components.Length || components[i] is null) continue;
            components[i].Enable();
        }
        if (st.stageDuration < 0) return;
        GlobalTimer.instance.AddTimer(new TimerRequest(st.stageDuration,ChangeStage));
    }


}
