using System.Reflection;
using UnityEngine;

// public fields, not properties: JsonUtility skips properties
public delegate void NotifySettingsChanged();
public class Settings
{
    public float UIFXVolume;
    public float GameFXVolume;
    public float BGMVolume;
    public bool JoystickMovement; // false = tap to move
    public float JoystickSensitivity;
    public event NotifySettingsChanged settingsChanged;

    public Settings()
    {
        UIFXVolume = 1;
        GameFXVolume = 1;
        BGMVolume = 1;
        JoystickSensitivity = 1;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }

    // save: false applies live without touching disk (e.g. mid-drag); call SaveManager.SaveSettings when done
    public void UpdateSetting(string setting, object value, bool save = true)
    {
        FieldInfo field = this.GetType().GetField(setting, BindingFlags.Instance | BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(this,value);
            settingsChanged?.Invoke();
            if (save) SaveManager.SaveSettings(this);
        }
    }
}
