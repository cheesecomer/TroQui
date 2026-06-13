using UnityEngine;

public class Preference {
    private const string HighScoreKey = "HighScore";

    private static Preference instance = new Preference();
    public static Preference Instance {
        get {
            return instance;
        }
    }

    private int highScore;
    public int HighScore {
        get { return this.highScore; }
        set {
            this.highScore = value;
            PlayerPrefs.SetInt(HighScoreKey, value);
            PlayerPrefs.Save();
        }
    }

    private Preference() {
        this.highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }
}