using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Text text;
    private int score;

    // Use this for initialization
    void Start() {
        score = 0;
        text.text = "" + this.score;
    }

    // Update is called once per frame
    void Update() {

    }

    public void IncreaseScore(int value) {
        this.score += value;
        text.text = "" + this.score;
    }
}
