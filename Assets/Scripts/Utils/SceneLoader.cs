using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void playButton()
    {
        TransitionManager.instance.startTransition(2);
    }

    public void exitButton()
    {
        Application.Quit();
    }

    public void menuButton()
    {
        TransitionManager.instance.startTransition(0);
    }

    public void shopButton()
    {
        TransitionManager.instance.startTransition(1);
    }
}
