using System;
using TMPro;
using UnityEngine;
public class StageManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private float stageNameDuration = 2f;


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

    private void HideStageName() => stageNameText.gameObject.SetActive(false);

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
        if (stageNameText != null)
        {
            stageNameText.text = st.name;
            stageNameText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideStageName));
            Invoke(nameof(HideStageName), stageNameDuration);
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
