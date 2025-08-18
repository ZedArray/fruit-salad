using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void playButton()
    {
        SceneManager.LoadScene(1);
    }
    
    public void exitButton()
    {
        Application.Quit();
    }
}
