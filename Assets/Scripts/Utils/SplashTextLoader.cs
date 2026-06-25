using TMPro;
using UnityEngine;

public class SplashTextLoader : MonoBehaviour
{
    [SerializeField] TextAsset txtFile;
    [SerializeField] TextMeshProUGUI splashTextUI;

    private string[] splashTexts;
    private float splashTimer;
    private float changeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splashTexts = txtFile.text.Split('\n');
        changeTime = 5f;
        splashTimer = changeTime;
    }

    // Update is called once per frame
    void Update()
    {
        splashTimer += Time.deltaTime;
        if (splashTimer >= changeTime)
        {
            splashTextUI.text = splashTexts[Random.Range(0, splashTexts.Length)];
            splashTimer = 0f;
        }
    }
}
