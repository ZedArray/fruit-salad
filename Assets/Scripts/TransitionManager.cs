using DentedPixel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    private int toTransition = 0;
    private bool transitioning; // ignores repeat calls (e.g. Loading calls every frame) until the scene loads

    [SerializeField]
    private Animator apple;

    [SerializeField]
    private float transitionTime;

    new void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnLoadScene;
    }

    void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        transitioning = false;
        anim();
    }

    public void startTransition(int n)
    {
        //if (n != SceneManager.GetActiveScene().buildIndex)
        //{
        //    toTransition = n;
        //    startAnim();
        //}
        if (transitioning) return;
        transitioning = true;
        toTransition = n;
        if (SFXManager.instance != null) SFXManager.instance.PlaySound("Transition");
        startAnim();
    }

    public void startAnim()
    {
        anim();
    }

    private void anim()
    {
        apple.SetTrigger("change");
    }

    public void changeScene()
    {
        SceneManager.LoadScene(toTransition);
    }
}
