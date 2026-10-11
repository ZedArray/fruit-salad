using UnityEngine;

// public fields, not properties: JsonUtility skips properties
public class Settings
{
    public float UIFXVolume;
    public float GameFXVolume;
    public float BGMVolume;
    
    public Settings()
    {
        UIFXVolume = 1;
        GameFXVolume = 1;
        BGMVolume = 1;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
