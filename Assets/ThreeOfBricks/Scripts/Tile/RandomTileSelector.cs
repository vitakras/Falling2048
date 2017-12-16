using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTileSelector : MonoBehaviour {

    public int[] selectableNumbers = new int[0];

    // Use this for initialization
    void Start() {

    }

    public int GetRandomTileNumber() {
        int value = Random.Range(0, selectableNumbers.Length);
        return selectableNumbers[value];
    }
}
