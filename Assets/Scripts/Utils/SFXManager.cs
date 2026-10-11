using UnityEngine;
using SysRand = System.Random;
public class SFXManager : Singleton<SFXManager>
{
    [SerializeField] private SFXProfile _sfxProfile;
    private SysRand _rng;

    protected override void Awake()
    {
        base.Awake();
        _rng = new SysRand(System.DateTime.Now.GetHashCode());
    }

    public void PlaySound(string key)
    {
        SFXData sfx = _sfxProfile.SFXLUT[key];
        AudioSource.PlayClipAtPoint(sfx.getClip(_rng.Next()), Camera.main.transform.position, VolumeFor(sfx.Category));
    }
    public void PlaySound(string key,Vector3 position)
    {
        SFXData sfx = _sfxProfile.SFXLUT[key];
        AudioSource.PlayClipAtPoint(sfx.getClip(_rng.Next()), position, VolumeFor(sfx.Category));
    }

    private static float VolumeFor(AudioCategory category)
    {
        Settings settings = SaveManager.LoadSettings();
        return category switch
        {
            AudioCategory.UIFX => settings.UIFXVolume,
            AudioCategory.BGM => settings.BGMVolume,
            _ => settings.GameFXVolume,
        };
    }
}
