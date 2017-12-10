using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameTile : MonoBehaviour {

    public int number;
    public Image background;
    public Text score;

    public int Number {
        get {
            return number;
        }
        set {
            number = value;
            this.score.text = string.Format("{0}", number);
        }
    }

    public void SetBackground(Sprite sprite) {
        background.sprite = sprite;
    }
    public void SetBackgroundColor(Color color) {
        background.color = color;
    }


    public void SetTextColor(Color color) {
        this.score.color = color;
    }
}
