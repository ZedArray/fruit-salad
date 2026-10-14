using System.Reflection;
using UnityEngine;

// public fields, not properties: JsonUtility skips properties
public delegate void NotifySettingsChanged();
public class Settings
{
    public float UIFXVolume;
    public float GameFXVolume;
    public float BGMVolume;
    public event NotifySettingsChanged settingsChanged;
    
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

    public void UpdateSetting(string setting, object value)
    {
        FieldInfo field = this.GetType().GetField(setting, BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(this,value);
            settingsChanged?.Invoke();
            SaveManager.SaveSettings(this);
        }
    }
}
