using UnityEngine;

public class Preference
{
    private const string HighScoreKey = "HighScore";

    private int _highScore;

    private Preference()
    {
        _highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public static Preference Instance { get; } = new();

    public int HighScore
    {
        get => _highScore;
        set
        {
            _highScore = value;
            PlayerPrefs.SetInt(HighScoreKey, value);
            PlayerPrefs.Save();
        }
    }
}