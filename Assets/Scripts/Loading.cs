using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] SpriteRenderer fruit;
    [SerializeField] Button button;

    private bool canLoad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.gameObject.SetActive(false);
        canLoad = false;
        StartCoroutine(LoadGame());
    }

    // Update is called once per frame
    void Update()
    {
        fruit.transform.Rotate(0, 0, 50 * Time.deltaTime);
    }

    IEnumerator LoadGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                button.gameObject.SetActive(true);
                if (canLoad)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    public void loadButton()
    {
        canLoad = true;
    }
}
