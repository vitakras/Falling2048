using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, INumberUpdateHandler {

    public GameObject numberedTilePrefab;
    public GameGrid gameGrid;
    public TileStyles style;

    private NumberTile activeTile;
    private Coroutine fall;
    private bool inputEnabled = true;
    private bool playerMovedTile = false;

    // Use this for initialization
    void Start() {
        CreateNumberedTiles();
        GetNewActiveTile();
        ResetFalling();
    }

    // Update is called once per frame
    void Update() {
        if (!inputEnabled) {
            return;
        }

        // We dont want player to controll the tiles if its inactive
        if (IsInactiveTile(activeTile)) {
            return;
        }

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
            playerMovedTile = true;
            activeTile.MoveToTile(tile);
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
        yield return FallActiveTile();
        NumberTile tile = activeTile;
        activeTile = null;
        List<NumberTile> tiles = MergeNeighbourTiles(tile);
        DeActivateAndDropTileAbove(tiles);
        GetNewActiveTile();
        ResetFalling();
    }

    IEnumerator FallActiveTile() {
        Debug.Log("Tile Started Falling");
        NumberTile neighbour = activeTile.FindNeighbourTile(Direction.down);
        while (IsInactiveTile(neighbour)) {
            yield return new WaitForSeconds(0.5f);
            if (!playerMovedTile) {
                activeTile.MoveToTile(neighbour);
            }
            neighbour = activeTile.FindNeighbourTile(Direction.down);
            playerMovedTile = false;
        }
        Debug.Log("Tile Finished Falling");
    }



    IEnumerator FallTile(NumberTile tile) {
        Debug.Log("Tile Started Falling");
        NumberTile neighbour = tile.FindNeighbourTile(Direction.down);
        while (IsInactiveTile(neighbour)) {
            yield return new WaitForSeconds(0.5f);
            tile.MoveToTile(neighbour);
            neighbour = tile.FindNeighbourTile(Direction.down);
        }
        Debug.Log("Tile Finished Falling");
    }

    IEnumerator FallAndMergeTile(NumberTile tile) {
        yield return FallTile(tile);
        List<NumberTile> tiles = MergeNeighbourTiles(tile);

        DeActivateAndDropTileAbove(tiles);
    }

    void DeActivateAndDropTileAbove(List<NumberTile> tiles) {
        foreach (NumberTile tile in tiles) {
            tile.DeActivate();
            NumberTile neighbour = tile.FindNeighbourTile(Direction.up);
            if (IsActiveTile(neighbour)) {
                StartCoroutine(FallAndMergeTile(neighbour));
            }
        }
    }

    List<NumberTile> MergeNeighbourTiles(NumberTile tile) {
        Direction[] directions = new Direction[] { Direction.left, Direction.right, Direction.down };
        List<NumberTile> tiles = new List<NumberTile>();

        foreach (Direction direction in directions) {
            NumberTile neighbour = tile.FindNeighbourTile(direction);
            if (IsActiveTile(neighbour) && tile.IsEqualNumber(neighbour)) {
                tiles.Add(neighbour);
            }
        }

        int multiplier = (int)Mathf.Pow(2, tiles.Count);
        tile.Multiply(multiplier);
        return tiles;
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

    void GetNewActiveTile(int number = 2) {
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

    bool IsInactiveTile(NumberTile tile) {
        return tile != null && !tile.Active;
    }
}
