using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class StageComponent : MonoBehaviour
{
    public bool doStage;
    public void enable()
    {
        doStage = true;
    }

    public void disable()
    {
        doStage = false;
    }
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public StageComponent[] stageManagers;
    
}

