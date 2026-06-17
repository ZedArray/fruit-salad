using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// DO NOT USE THIS 
/// </summary>
public abstract class StageComponentNG : MonoBehaviour
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

public abstract class StageComponent<T> : StageComponentNG
    where T:StageComponent<T>
{
    
    public static T instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            
        }
        else
        {
            Destroy(this);
        }
    }
}

[Serializable]
public struct StageDefinition
{
    public int[] enabledStages;
    [Tooltip("Any value below 0 implies an infinitely long stage.")]
    public int stageDuration;
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public string[] stageManagersClassNames;
    public StageDefinition[] stages;


}

