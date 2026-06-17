using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class StageComponent : MonoBehaviour
{
    public bool doStage;
    public virtual void Enable()
    {
        doStage = true;
    }

    public virtual void Disable()
    {
        doStage = false;
    }
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public StageComponent[] stageManagers;
    
}

