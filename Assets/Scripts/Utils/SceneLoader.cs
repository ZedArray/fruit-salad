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
        SceneManager.LoadScene(1);
    }
    
    public void exitButton()
    {
        Application.Quit();
    }
}
