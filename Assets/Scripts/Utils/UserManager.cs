using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    [SerializeField] private bool[] skinUnlock;

    private int coinAmount;
    private int skinID;

    public Settings settings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // change these two to grab from the cloud save or smth
        coinAmount = 0;
        skinID = 0;
        

    }

    protected override void Awake()
    {
        base.Awake();
        settings = SaveManager.LoadSettings();
    }

    public int getCoinAmount()
    {
        return coinAmount;
    }

    public int getSkinID()
    {
        return skinID;
    }

    public bool[] getSkinUnlock()
    {
        return skinUnlock;
    }

    public bool getSkinUnlockID(int n)
    {
        return skinUnlock[n];
    }

    public void unlockSkinID(int n)
    {
        skinUnlock[n] = true;
    }

    public void setCoinAmount(int n)
    {
        coinAmount = n;
    }

    public void addCoinAmount(int n)
    {
        coinAmount += n;
    }

    public void setSkinID(int n)
    {
        skinID = n;
    }
}
