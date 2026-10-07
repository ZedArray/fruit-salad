using System;
using System.Text;
using UnityEngine;

// base64-encoded JSON kept in PlayerPrefs; add fields to Data as more needs saving
public static class SaveData
{
    [Serializable]
    class Data
    {
        public int highScore;
        public bool highScorePending; // best not yet accepted by the leaderboard
    }

    const string Key = "save";
    static Data data;

    static Data Current => data ??= Load();

    public static int HighScore
    {
        get => Current.highScore;
        set
        {
            Current.highScore = value;
            Save();
        }
    }

    public static bool HighScorePending
    {
        get => Current.highScorePending;
        set
        {
            Current.highScorePending = value;
            Save();
        }
    }

    static Data Load()
    {
        try
        {
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(PlayerPrefs.GetString(Key)));
            return JsonUtility.FromJson<Data>(json) ?? new Data();
        }
        catch (Exception)
        {
            // missing or corrupt save starts fresh
            return new Data();
        }
    }

    static void Save()
    {
        PlayerPrefs.SetString(Key, Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.ToJson(data))));
        PlayerPrefs.Save();
    }
}
