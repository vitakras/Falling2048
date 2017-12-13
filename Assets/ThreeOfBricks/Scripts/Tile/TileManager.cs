using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, INumberUpdateHandler {

    public GameObject numberedTilePrefab;
    public GameGrid gameGrid;
    public TileStyles style;

    private NumberTile activeTile;
    private Coroutine fall;

    // Use this for initialization
    void Start() {
        CreateNumberedTiles();
        GetNewActiveTile();
        ResetFalling();
    }


    // Update is called once per frame
    void Update() {
        NumberTile tile = null;
        if (Input.GetKeyDown(KeyCode.A)) {
            tile = activeTile.FindNeighbourTile(Direction.left);
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            tile = activeTile.FindNeighbourTile(Direction.right);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            tile = FindFloorTile(activeTile);
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
                NumberTileView numberTile = tile.GetComponent<NumberTileView>();
                numberTile.UpdateHandler = this;
                cells[i, j].SetChild(tile, false);
            }
        }
    }

    IEnumerator Fall() {
        Debug.Log("Created");
        NumberTile tile = activeTile.FindNeighbourTile(Direction.down);
        while (tile != null && !tile.Active) {
            yield return new WaitForSeconds(1);
            activeTile.MoveToTile(tile);
            tile = activeTile.FindNeighbourTile(Direction.down);
        }

        Debug.Log("Done Falling");
        MergeTiles();
        GetNewActiveTile();
        ResetFalling();
    }

    void MergeTiles() {
        List<NumberTile> tiles = FindEqualTilesInDirection(activeTile, Direction.left);
        tiles.AddRange(FindEqualTilesInDirection(activeTile, Direction.right));
        tiles.AddRange(FindEqualTilesInDirection(activeTile, Direction.down));

        if (tiles.Count == 0) {
            return;
        }

        int multiplier = (int)Mathf.Pow(2, tiles.Count);
        activeTile.Number = (multiplier * activeTile.Number);
    }

    void ResetFalling() {
        if (fall != null) {
            StopCoroutine(fall);
        }

        fall = StartCoroutine(Fall());
    }

    NumberTile FindFloorTile(NumberTile tile) {
        NumberTile nextTile = tile.FindNeighbourTile(Direction.down);
        while (nextTile != null && !nextTile.Active) {
            tile = nextTile;
            nextTile = tile.FindNeighbourTile(Direction.down);
        }

        return tile;
    }

    List<NumberTile> FindEqualTilesInDirection(NumberTile tile, Direction dir) {
        List<NumberTile> tiles = new List<NumberTile>();
        NumberTile neighbourTile = tile.FindNeighbourTile(dir);
        while (IsActiveTile(neighbourTile)) {
            if (neighbourTile.Number != tile.Number) {
                break;
            }

            tiles.Add(neighbourTile);
            neighbourTile = neighbourTile.FindNeighbourTile(dir);
        }

        return tiles;
    }

    void GetNewActiveTile(int number = 2048) {
        activeTile = new NumberTile(gameGrid, new CellPosition(2, 0)) {
            Active = true
        };
        activeTile.Number = number;
    }

    public void OnNumberUpdated(NumberTileView numberTile) {
        this.style.ApplyStyle(numberTile);
    }

    bool IsActiveTile(NumberTile tile) {
        return tile != null && tile.Active;
    }
}
