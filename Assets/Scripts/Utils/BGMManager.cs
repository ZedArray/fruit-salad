using System;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(AudioSource))]
public class BGMManager : Singleton<BGMManager>
{
    private int _currScene = 0;
    [SerializeField] private AudioClip[] clips;
    private int nclips;
    private AudioSource _source;
    new void Awake()
    {
        base.Awake();
        _source = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLoadScene;
        _source.loop = true;
        nclips = clips.Length;
    }

    private void Start()
    {
        setVolume();
        UserManager.instance.settings.settingsChanged += setVolume;
        changeBGM();
    }

    void changeBGM()
    {
        if (clips[_currScene % nclips] == _source.clip)
        {
            return;
        }

        _source.Stop();
        _source.clip = clips[_currScene % nclips];
        _source.Play();
    }

    void setVolume()
    {
        _source.volume = UserManager.instance.settings.BGMVolume;
    }

    void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        print("Attempting Music Change");
        _currScene = scene.buildIndex;
        changeBGM();
    }
}
