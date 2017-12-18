using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour, INumberUpdateHandler {

    public GameObject numberedTilePrefab;
    public GameGrid gameGrid;
    public TileStyles style;
    public TileManager tileManager;

    public UnityEvent gameStarted = new UnityEvent();
    public UnityEvent gamePaused = new UnityEvent();
    public UnityEvent gameResumed = new UnityEvent();
    public UnityEvent gameEnded = new UnityEvent();

    // Use this for initialization
    void Start() {
        gameGrid.Initialize();
        PopulateGridWithNumberedTiles();
    }

    public void StartGame() {
        EnableTileManager();
        gameStarted.Invoke();
    }

    public void ResumeGame() {
        EnableTileManager();
        gameResumed.Invoke();
    }

    public void PauseGame() {
        DisableTileManager();
        gamePaused.Invoke();
    }

    public void EndGame() {
        DisableTileManager();
        gameEnded.Invoke();
    }

    public void ResetGame() {
        tileManager.ResetFallingTile();
        ClearGrid();
    }

    public void OnNumberUpdated(NumberTileView numberTile) {
        this.style.ApplyStyle(numberTile);
    }

    void PopulateGridWithNumberedTiles() {
        Cell[,] cells = gameGrid.GetAllCells();
        for (int j = 0; j < gameGrid.columns; j++) {
            for (int i = 0; i < gameGrid.rows; i++) {
                cells[i, j].SetChild(InstantiateNumberedTile(), false);
            }
        }
    }

    GameObject InstantiateNumberedTile() {
        GameObject tile = GameObject.Instantiate(numberedTilePrefab);
        NumberTileView numberTile = tile.GetComponent<NumberTileView>();
        numberTile.UpdateHandler = this;

        return tile;
    }

    private void ClearGrid() {
        Cell[,] cells = gameGrid.GetAllCells();
        for (int j = 0; j < gameGrid.columns; j++) {
            for (int i = 0; i < gameGrid.rows; i++) {
                cells[i, j].SetChildActive(false);
            }
        }
    }

    void DisableTileManager() {
        tileManager.DisableControl();
        tileManager.StopFallingTile();
    }

    void EnableTileManager() {
        tileManager.EnableControl();
        tileManager.StartFallingTile();
    }
}
