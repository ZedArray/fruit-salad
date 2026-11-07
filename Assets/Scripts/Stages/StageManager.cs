using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
public class StageManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private float stageNameDuration = 2f;
    [SerializeField] private float typeDelay = 0.04f;
    [SerializeField] private float eraseDelay = 0.015f;
    [SerializeField] private Babble babble;
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
        if (stageNameText != null) stageNameText.gameObject.SetActive(false);
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

    // types the line out with babble, holds, then backspaces it away
    private IEnumerator ShowStageName(string line)
    {
        stageNameText.text = line;
        stageNameText.maxVisibleCharacters = 0;
        stageNameText.gameObject.SetActive(true);
        stageNameText.ForceMeshUpdate();
        int count = stageNameText.textInfo.characterCount;
        char[] chars = new char[count]; // copied: textInfo is rebuilt as visibility changes
        for (int i = 0; i < count; i++) chars[i] = stageNameText.textInfo.characterInfo[i].character;

        for (int i = 0; i < count; i++)
        {
            stageNameText.maxVisibleCharacters = i + 1;
            if (babble != null) babble.Say(chars[i]);
            yield return new WaitForSeconds(".,!?\u2026".IndexOf(chars[i]) >= 0 ? typeDelay * 6 : typeDelay);
        }
        yield return new WaitForSeconds(stageNameDuration);
        for (int i = count - 1; i >= 0; i--)
        {
            stageNameText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(eraseDelay);
        }
        stageNameText.gameObject.SetActive(false);
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
            string line = st.lines is { Length: > 0 } ? st.lines[Random.Range(0, st.lines.Length)] : st.name;
            stageNameRoutine = StartCoroutine(ShowStageName(line));
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
