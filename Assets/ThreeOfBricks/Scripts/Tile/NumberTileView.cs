using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NumberTileView : MonoBehaviour {

    public int number;
    public Image background;
    public Text score;
    public INumberUpdateHandler updateHandler;

    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
    }

    public int Number {
        get {
            return number;
        }
        set {
            number = value;
            this.score.text = string.Format("{0}", number);
            if (updateHandler != null) {
                updateHandler.OnNumberUpdated(this);
            }
        }
    }

    public INumberUpdateHandler UpdateHandler {
        set {
            updateHandler = value;
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

    public void PlayUpdateAnimation() {
        anim.SetTrigger("Update");
    }
}

public interface INumberUpdateHandler {
    void OnNumberUpdated(NumberTileView numberTile);
}
