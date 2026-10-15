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

    public static float VolumeFor(AudioCategory category)
    {
        
        return category switch
        {
            AudioCategory.UIFX => UserManager.instance.settings.UIFXVolume,
            AudioCategory.BGM => UserManager.instance.settings.BGMVolume,
            _ => UserManager.instance.settings.GameFXVolume,
        };
    }
}
