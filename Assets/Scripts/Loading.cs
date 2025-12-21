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
    async void Start()
    {
        button.gameObject.SetActive(false);
        canLoad = false;
       LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        fruit.transform.Rotate(0, 0, 50 * Time.deltaTime);
    }

    async void LoadGame()
    {
        SceneManager.LoadScene("MainScene");

    }

    public void loadButton()
    {
        canLoad = true;
    }
}
