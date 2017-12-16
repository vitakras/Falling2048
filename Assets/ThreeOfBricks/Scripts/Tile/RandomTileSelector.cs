using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTileSelector : MonoBehaviour, INumberUpdateHandler {

    public NumberTileView numberTileView;
    public int[] selectableNumbers = new int[0];
    public TileStyles style;

    int value = 0;

    // Use this for initialization
    void Start() {
        numberTileView.updateHandler = this;
        UpdateRandomNumber();
    }

    public int GetRandomTileNumber() {
        int value = this.value;
        UpdateRandomNumber();
        return value;
    }

    void UpdateRandomNumber() {
        int position = Random.Range(0, selectableNumbers.Length);
        value = selectableNumbers[position];

        numberTileView.Number = value;
    }

    public void OnNumberUpdated(NumberTileView numberTile) {
        style.ApplyStyle(numberTile);
    }
}
