using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scoreCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI billScore;
    [SerializeField] TextMeshProUGUI billCoin;
    [SerializeField] GameObject billObject;

    public int score;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        billObject.SetActive(false);
        score = 0;
    }

    public void TryAddScore()
    {
        if (Fruit.dead)
        {
            return;
        }

        score++;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void gameOver()
    {
        billObject.SetActive(true);
        billScore.text = score.ToString();
        billCoin.text = "+" + Fruit.instance.getCoinCaught().ToString();
    }

    public void restart()
    {
        TransitionManager.instance.startTransition(3);
    }

    public void menu()
    {
        TransitionManager.instance.startTransition(0);
    }
}
