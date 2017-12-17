using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    public void StartGame() {
        Debug.Log("Game Started");
    }

    public void ResumeGame() {
        Debug.Log("Game Resumed");
    }

    public void PauseGame() {
        Debug.Log("Game Paused");
    }

    public void EndGame() {
        Debug.Log("Game Ended");
    }

    public void ResetGame() {
        Debug.Log("Game Reset");
    }
}
