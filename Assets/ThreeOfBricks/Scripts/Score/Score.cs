using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Score : MonoBehaviour {

    public UnityEvent onScoreUpdated = new UnityEvent();

    private int score;
    private int highscore;
    private const string scoreString = "score";

    // Use this for initialization
    void Awake() {
        Load();
        ResetScore();
    }

    public int GameScore {
        set {
            score = value;

            if (score > highscore) {
                highscore = score;
            }

            onScoreUpdated.Invoke();
        }
        get {
            return score;
        }
    }

    public void IncreaseScore(int value) {
        GameScore = GameScore + value;
    }

    public int GetHighScore() {
        return this.highscore;
    }

    public void ResetScore() {
        this.GameScore = 0;
    }

    public void ResetHighScore() {
        this.highscore = 0;
    }

    public void Load() {
        highscore = PlayerPrefs.GetInt(scoreString, 0);
    }

    public void Save() {
        PlayerPrefs.SetInt(scoreString, highscore);
    }
}
