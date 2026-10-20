using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private DataLoader dl;
    public int[] skinPrice;

    public void purchaseSkin(int skinId)
    {
        int coinAmount = dl.getCoinAmount();

        if (skinPrice[skinId] > coinAmount) return;

        dl.changeCoinAmount(-skinPrice[skinId]);
        dl.unlockSkinID(skinId);
    }
}
