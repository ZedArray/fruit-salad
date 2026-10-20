using System.IO;
using UnityEditor;
using UnityEngine;

// Tools > Check Wallet: asserts the shop/wallet rules, leaves your real wallet.json untouched
static class WalletCheck
{
    [MenuItem("Tools/Check Wallet")]
    static void Run()
    {
        var w = new SaveManager.WalletData { coins = 30 };
        Debug.Assert(!w.Owns("ghost") && w.skin == "", "starts with nothing, default skin");
        Debug.Assert(!w.TryBuy("red", 50) && w.coins == 30 && !w.Owns("red"), "can't afford");
        Debug.Assert(w.TryBuy("ghost", 25) && w.coins == 5 && w.Owns("ghost") && w.skin == "ghost", "buy deducts, owns, equips");
        Debug.Assert(w.TryBuy("ghost", 25) && w.coins == 5, "rebuy owned is free");
        Debug.Assert(w.TryBuy("apple", 0) && w.skin == "apple" && w.coins == 5, "free item equips");
        Debug.Assert(w.TryBuy("cloth1", 0, table: true) && w.table == "cloth1" && w.skin == "apple", "tablecloth equips without touching the skin");
        Debug.Assert(w.TryBuy("ghost", 0) && w.skin == "ghost" && w.table == "cloth1", "skin equips without touching the tablecloth");

        string path = Path.Combine(Application.persistentDataPath, "wallet.json");
        string backup = File.Exists(path) ? File.ReadAllText(path) : null;
        try
        {
            SaveManager.SaveWallet(w);
            Debug.Assert(SaveManager.LoadWallet().coins == 5, "round trip");
            File.WriteAllText(path, File.ReadAllText(path).Replace("\"coins\":5", "\"coins\":9999"));
            Debug.Assert(SaveManager.LoadWallet().coins == 0, "tampered save resets");
        }
        finally
        {
            if (backup != null) File.WriteAllText(path, backup);
            else File.Delete(path);
        }
        Debug.Log("Wallet check done (any failures logged above)");
    }
}
