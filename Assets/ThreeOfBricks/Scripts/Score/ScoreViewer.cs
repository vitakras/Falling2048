using UnityEngine;
using UnityEngine.UI;

public class ScoreViewer : MonoBehaviour {

    public bool highscore;
    public Score score;
    public Text scoreText;

    void OnEnable() {
        UpdateScore();
    }

    public void UpdateScore() {
        if (score == null) {
            return;
        }

        if (highscore) {
            scoreText.text = string.Format("{0}", score.GetHighScore());
        }
        else {
            scoreText.text = string.Format("{0}", score.GameScore);
        }
    }
}
