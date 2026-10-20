using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    // ponytail: local save only, move to Cloud Save if accounts should carry coins across devices
    private SaveManager.WalletData wallet;

    public Settings settings;

    protected override void Awake()
    {
        base.Awake();
        settings = SaveManager.LoadSettings();
        wallet = SaveManager.LoadWallet();
    }

    public int getCoinAmount()
    {
        return wallet.coins;
    }

    // ShopCatalog item id, "" = default skin
    public string getSkinID()
    {
        return wallet.skin;
    }

    // ShopCatalog item id, "" = default tablecloth
    public string getTableID()
    {
        return wallet.table;
    }

    public string getEquippedID(ShopCatalog catalog)
    {
        return catalog.table ? wallet.table : wallet.skin;
    }

    public bool owns(string id)
    {
        return wallet.Owns(id);
    }

    public void addCoinAmount(int n)
    {
        wallet.coins += n;
        SaveManager.SaveWallet(wallet);
    }

    public bool tryBuy(ShopCatalog catalog, ShopCatalog.Item item)
    {
        if (!wallet.TryBuy(item.id, item.price, catalog.table)) return false;
        SaveManager.SaveWallet(wallet);
        return true;
    }

    [ContextMenu("Add 100 Coins")]
    private void addTestCoins()
    {
        addCoinAmount(100);
    }
}
