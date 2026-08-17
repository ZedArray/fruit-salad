using DentedPixel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    private int toTransition = 0;

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
        anim();
    }

    public void startTransition(int n)
    {
        //if (n != SceneManager.GetActiveScene().buildIndex)
        //{
        //    toTransition = n;
        //    startAnim();
        //}
        toTransition = n;
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
