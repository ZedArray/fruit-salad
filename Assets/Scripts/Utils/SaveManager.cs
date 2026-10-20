using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// one JSON file per saved thing in persistentDataPath; add a Save/Load pair per thing
public static class SaveManager
{
    [Serializable]
    class HighScoreData
    {
        public int highScore;
        public long setAt; // unix ms when the high score was set
        public string hash; 
        public bool pending; // best not yet accepted by the leaderboard
    }

    [Serializable]
    public class WalletData
    {
        public int coins;
        public List<string> owned = new List<string>(); // ShopCatalog item ids, unique across all catalogs
        public string skin = ""; // equipped item id, "" = default
        public string table = ""; // equipped tablecloth id, "" = default
        public string hash;

        public bool Owns(string id) => owned.Contains(id);

        // buys and equips (into the table slot if table); owned (or free) items just get equipped
        public bool TryBuy(string id, int price, bool table = false)
        {
            if (!Owns(id))
            {
                if (coins < price) return false;
                coins -= price;
                owned.Add(id);
            }
            if (table) this.table = id;
            else skin = id;
            return true;
        }
    }

    const string HighScoreFile = "highscore.json";
    const string SettingsFile = "settings.json";
    const string WalletFile = "wallet.json";
    static readonly byte[] HashKey = Encoding.UTF8.GetBytes("fruit-salad/highscore/v1");

    static HighScoreData highScore;
    static HighScoreData CurrentHighScore => highScore ??= ReadHighScore();

    static Settings settings;

    // ---- high score ----

    public static int LoadHighScore() => CurrentHighScore.highScore;

    public static bool LoadHighScorePending() => CurrentHighScore.pending;

    // a new best is always unsubmitted, so this also marks it pending
    public static void SaveHighScore(int score)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        highScore = new HighScoreData { highScore = score, setAt = now, hash = Hash(score, now), pending = true };
        Write(HighScoreFile, JsonUtility.ToJson(highScore));
    }

    public static void SaveHighScorePending(bool pending)
    {
        CurrentHighScore.pending = pending;
        Write(HighScoreFile, JsonUtility.ToJson(highScore));
    }

    static HighScoreData ReadHighScore()
    {
        var data = Read<HighScoreData>(HighScoreFile);
        if (data == null) return new HighScoreData();
        if (data.hash == Hash(data.highScore, data.setAt)) return data;

        Debug.LogWarning("High score save failed verification, resetting");
        Delete(HighScoreFile);
        return new HighScoreData();
    }

    static string Hash(int score, long setAt) => Hash($"{score}:{setAt}");

    static string Hash(string payload)
    {
        using var hmac = new HMACSHA256(HashKey);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
    }

    // ---- wallet (coins + skins) ----

    static string WalletHash(WalletData w) => Hash($"wallet:{w.coins}:{w.skin}:{w.table}:{string.Join(",", w.owned)}");

    public static WalletData LoadWallet()
    {
        var data = Read<WalletData>(WalletFile);
        if (data == null) return new WalletData();
        if (data.hash == WalletHash(data)) return data;

        Debug.LogWarning("Wallet save failed verification, resetting");
        Delete(WalletFile);
        return new WalletData();
    }

    public static void SaveWallet(WalletData wallet)
    {
        wallet.hash = WalletHash(wallet);
        Write(WalletFile, JsonUtility.ToJson(wallet));
    }

    // ---- settings ----

    // shared instance; change it then call SaveSettings
    public static Settings LoadSettings() => settings ??= Read<Settings>(SettingsFile) ?? new Settings();

    public static void SaveSettings(Settings value)
    {
        settings = value;
        Write(SettingsFile, value.ToString());
    }

    // ---- file helpers ----

    static string PathOf(string file) => Path.Combine(Application.persistentDataPath, file);

    // null when missing or unreadable; fields absent from the file keep their constructor defaults
    static T Read<T>(string file) where T : class, new()
    {
        try
        {
            string path = PathOf(file);
            if (!File.Exists(path)) return null;
            var data = new T();
            JsonUtility.FromJsonOverwrite(File.ReadAllText(path), data);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Couldn't read {file}: {e.Message}");
            return null;
        }
    }

    static void Write(string file, string json)
    {
        // write to a temp file then swap, so a crash mid-write can't corrupt the old save
        string path = PathOf(file), tmp = path + ".tmp";
        try
        {
            File.WriteAllText(tmp, json);
            if (File.Exists(path)) File.Replace(tmp, path, null);
            else File.Move(tmp, path);
        }
        catch (Exception e)
        {
            Debug.LogError($"Couldn't write {file}: {e.Message}");
        }
    }

    static void Delete(string file)
    {
        try { File.Delete(PathOf(file)); }
        catch (Exception e) { Debug.LogWarning($"Couldn't delete {file}: {e.Message}"); }
    }
}
