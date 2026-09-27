using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    private int coinAmount;
    private int skinID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // change these two to grab from the cloud save or smth
        coinAmount = 0;
        skinID = 0;


    }

    public int getCoinAmount()
    {
        return coinAmount;
    }

    public int getSkinID()
    {
        return skinID;
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
