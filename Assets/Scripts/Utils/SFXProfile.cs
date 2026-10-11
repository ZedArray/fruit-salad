using UnityEngine;
using System.Collections.Generic;

// picks which Settings volume a sound plays at; GameFX first so existing entries default to it
// (not named AudioType: that clashes with UnityEngine.AudioType)
public enum AudioCategory
{
    GameFX,
    UIFX,
    BGM
}

[System.Serializable]
public struct SFXData
{
    public AudioClip[] AudioClips;
    public AudioCategory Category;

    public AudioClip getClip(int n)
    {
        return AudioClips[n % AudioClips.Length];
    }
}

[System.Serializable]
public struct SFXKVP
{
    public string name;
    public SFXData clips;
}

[CreateAssetMenu(fileName = "SFXProfile", menuName = "Scriptable Objects/SFXProfile")]
public class SFXProfile : ScriptableObject
{
    [SerializeField] private SFXKVP[] _SFXLUT;
    public Dictionary<string, SFXData> SFXLUT = new Dictionary<string, SFXData>();

    // dictionaries aren't serialized, so rebuild it whenever the asset loads
    private void OnEnable() => Populate();

    [ContextMenu("Populate Dictionary")]
    private void Populate()
    {
        SFXLUT.Clear();
        foreach (var kvp in _SFXLUT)
        {
            SFXLUT.Add(kvp.name, kvp.clips);
        }
    }
}
