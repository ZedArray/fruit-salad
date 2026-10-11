using System;
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
        public string hash; // HMAC of highScore + setAt, catches hand-edited saves
        public bool pending; // best not yet accepted by the leaderboard
    }

    const string HighScoreFile = "highscore.json";
    const string SettingsFile = "settings.json";
    // ponytail: key ships in the binary, so this stops casual edits, not a determined decompiler
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

    static string Hash(int score, long setAt)
    {
        using var hmac = new HMACSHA256(HashKey);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{score}:{setAt}")));
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
