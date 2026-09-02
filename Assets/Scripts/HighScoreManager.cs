using System;
using System.IO;
using UnityEngine;

public static class HighScoreManager
{
    private static string HighScorePath => Path.Combine(Application.persistentDataPath, "highscore.json");

    //----------------------------------------------------------------------------------------

    [Serializable]
    private class HighScoreData
    {
        public int highScore;
    }

    //----------------------------------------------------------------------------------------

    public static int Load()
    {
        if (!File.Exists(HighScorePath)) return 0;

        var json = File.ReadAllText(HighScorePath);
        var data = JsonUtility.FromJson<HighScoreData>(json);

        return data.highScore;
    }

    public static bool TrySave(int score)
    {
        var highScore = Load();
        if (score <= highScore) return false;

        var data = new HighScoreData()
        {
            highScore = score
        };

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(HighScorePath, json);
        return true;
    }
}