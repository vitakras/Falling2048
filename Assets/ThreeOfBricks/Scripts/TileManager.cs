using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, INumberUpdateHandler {

    public GameObject numberedTilePrefab;
    public GameGrid gameGrid;
    public TileStyles style;

    private Tile activeTile;
    private Coroutine fall;

    // Use this for initialization
    void Start() {
        CreateNumberedTiles();
        GetNewActiveTile();
        ResetFalling();
    }


    // Update is called once per frame
    void Update() {
        Tile tile = null;
        if (Input.GetKeyDown(KeyCode.A)) {
            tile = activeTile.FindNeighbourTile(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            tile = activeTile.FindNeighbourTile(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            tile = FindFloor(activeTile);
        }

        if (tile != null) {
            activeTile.MoveToTile(tile);
            ResetFalling();
        }
    }

    void CreateNumberedTiles() {
        Cell[,] cells = gameGrid.GetAllCells();
        for (int j = 0; j < gameGrid.columns; j++) {
            for (int i = 0; i < gameGrid.rows; i++) {
                GameObject tile = GameObject.Instantiate(numberedTilePrefab);
                NumberTile numberTile = tile.GetComponent<NumberTile>();
                numberTile.UpdateHandler = this;
                cells[i, j].SetChild(tile, false);
            }
        }
    }

    IEnumerator Fall() {
        Debug.Log("Created");
        Tile tile = activeTile.FindNeighbourTile(Vector2.down);
        while (tile != null && !tile.ActiveTile) {
            yield return new WaitForSeconds(1);
            activeTile.MoveToTile(tile);
            tile = activeTile.FindNeighbourTile(Vector2.down);
        }

        Debug.Log("Done Falling");
        GetNewActiveTile();
        ResetFalling();
    }

    void ResetFalling() {
        if (fall != null) {
            StopCoroutine(fall);
        }

        fall = StartCoroutine(Fall());
    }

    Tile FindFloor(Tile tile) {
        Tile nextTile = tile.FindNeighbourTile(Vector2.down);
        while (nextTile != null && !nextTile.ActiveTile) {
            tile = nextTile;
            nextTile = tile.FindNeighbourTile(Vector2.down);
        }

        return tile;
    }

    void GetNewActiveTile(int number = 2048) {
        activeTile = new Tile(gameGrid, new CellPosition(2, 0)) {
            ActiveTile = true
        };
        activeTile.SetNumber(number);
    }

    public void OnNumberUpdated(NumberTile numberTile) {
        this.style.ApplyStyle(numberTile);
    }
}
