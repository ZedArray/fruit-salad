using System;
using System.Reflection;
using UnityEngine;
using Microsoft.CSharp;
public class StageManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;


    private StageComponentNG[] components;
    private int stage;
    private int stageAmount;

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

    public void ChangeStage()
    {
        stage = (stage + 1) % stageAmount;
        StageDefinition st = stageData.stages[stage];
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
