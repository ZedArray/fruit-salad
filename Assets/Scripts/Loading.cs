using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] SpriteRenderer fruit;
    [SerializeField] Button button;
    [SerializeField] Slider progress;

    private float progressAmount;
    private bool canLoad;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progress.value = 0f;
        button.gameObject.SetActive(false);
        canLoad = false;
        StartCoroutine(LoadGame());
    }

    // Update is called once per frame
    void Update()
    {
        fruit.transform.Rotate(0, 0, 50 * Time.deltaTime);
        progressBar();
    }
    
    void progressBar()
    {
        float progVal = progress.value;

        if (progressAmount >= 0.9f)
        {
            progressAmount = 1f;
        }
        if (progVal == 1f)
        {
            //button.gameObject.SetActive(true);
            canLoad = true;
            TransitionManager.instance.startTransition(3);
        }

        //progress.value = Mathf.Lerp(0, progressAmount, 1f);
        progress.value = Mathf.Lerp(0, progressAmount, progVal + 1f * Time.deltaTime);
        //progress.value = progressAmount;
    }

    IEnumerator LoadGame()
    {
        while (true)
        {
            progressAmount += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
        //progressAmount += 1f;
        //yield return null;
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        //asyncLoad.allowSceneActivation = false;

        //while (!asyncLoad.isDone)
        //{
        //    //asyncLoad.allowSceneActivation = canLoad;
        //    progressAmount = asyncLoad.progress;
        //    yield return null;
        //}
    }

    public void loadButton()
    {
        canLoad = true;
    }
}
