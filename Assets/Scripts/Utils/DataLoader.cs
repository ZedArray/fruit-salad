using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounter;
    [SerializeField] private GameObject[] skinLock;

    private bool[] skinUnlock;
    private int coinAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadCoin();
        loadSkinUnlock();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setCoinText()
    {
        coinCounter.text = coinAmount.ToString();
    }

    private void setUserCoin()
    {
        UserManager.instance.setCoinAmount(coinAmount);
    }

    public void changeCoinAmount(int diff)
    {
        coinAmount += diff;
        setCoinText();
        setUserCoin();
    }

    public int getCoinAmount()
    {
        return coinAmount;
    }

    private void loadCoin()
    {
        print(coinAmount);
        coinAmount = UserManager.instance.getCoinAmount();
        setCoinText();
        print(coinAmount);
    }

    public void loadSkinUnlock()
    {
        skinUnlock = UserManager.instance.getSkinUnlock();
        for (int i = 0; i < skinUnlock.Length; i++)
        {
            //print(skinUnlock[i]);
            skinLock[i].gameObject.SetActive(!skinUnlock[i]);
        }
    }

    public void unlockSkinID(int skinId)
    {
        UserManager.instance.unlockSkinID(skinId);
        //loadSkinUnlock();
        skinUnlock[skinId] = true;
        skinLock[skinId].gameObject.SetActive(false);
    }

    public void loadSkinID(int n)
    {
        UserManager.instance.setSkinID(n);
    }
}
