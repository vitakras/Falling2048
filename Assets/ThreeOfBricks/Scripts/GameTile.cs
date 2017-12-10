using UnityEngine;
using UnityEngine.UI;

public class GameTile : MonoBehaviour {
    public Image background;
    public Text score;

    public void SetScore(int score) {
        this.score.text = string.Format("{0}",score);
    }

    public void SetColor(Color color) {
        background.color = color;
    }

    public void SetBackground(Sprite sprite) {
        background.sprite = sprite;
    }
}
