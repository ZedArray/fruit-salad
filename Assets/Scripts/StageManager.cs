using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] BladeSpawner bs;
    [SerializeField] UtensilSpawner us;
    [SerializeField] float[] stageStart;

    private int stage;
    private float timer;
    private int stageAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stage = 0;
        timer = 0f;
        bs.enabled = false;
        us.enabled = false;
        stageAmount = stageStart.Length;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (stage < stageAmount - 1)
        {
            timeCheck();
            stageChanger();
        }
    }

    void stageChanger()
    {
        switch (stage)
        {
            case 0:
                bs.enabled = true;
                us.enabled = false;
                break;
            case 1:
                bs.enabled = false;
                us.enabled = true;
                break;
            case 2:
                bs.enabled = true;
                us.enabled = true;
                break;
            default:
                break;
        }
    }

    void timeCheck()
    {
        if (timer > stageStart[stage + 1])
        {
            stage += 1;
        }
    }
}
