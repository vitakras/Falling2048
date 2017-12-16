using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, INumberUpdateHandler {

    public CellPosition activeTileSpawnPosition = new CellPosition(2, 0);
    public GameObject numberedTilePrefab;
    public GameGrid gameGrid;
    public TileStyles style;
    public InputHandler inputHandler;

    private NumberTile activeTile;
    private Coroutine fall;
    private bool inputEnabled = true;
    private bool playerMovedTile = false;
    private Queue<Coroutine> fallingTilesQueue;
    private readonly Direction[] mergeDirections = new Direction[] { Direction.left, Direction.right, Direction.down };

    // Use this for initialization
    void Start() {
        fallingTilesQueue = new Queue<Coroutine>();
        PopulateGridWithNumberedTiles();
        GetNewActiveTile();
        ResetFalling();
    }

    // Update is called once per frame
    void Update() {
        if (!inputEnabled || IsInactiveTile(activeTile)) {
            return;
        }
        HandlePlayerInput();
    }

    public void OnNumberUpdated(NumberTileView numberTile) {
        this.style.ApplyStyle(numberTile);
    }

    void HandlePlayerInput() {
        Direction direction = inputHandler.GetDirection();
        NumberTile tile = null;
        switch (direction) {
            case Direction.left:
            case Direction.right:
                tile = activeTile.FindNeighbourTile(direction);
                break;
            case Direction.down:
                tile = FindFloorTile(activeTile);
                break;
            default:
                return;
        }

        if (tile != null) {
            playerMovedTile = true;
            activeTile.MoveToTile(tile);
        }
    }

    void ResetFalling() {
        if (fall != null) {
            StopCoroutine(fall);
        }

        fall = StartCoroutine(MakeActiveTileFall());
    }

    void GetNewActiveTile() {
        activeTile = new NumberTile(gameGrid, activeTileSpawnPosition) {
            Active = true
        };
        //TODO Set Tile From NumberSelect 
        activeTile.Number = 2;
    }

    NumberTile ClearActiveTile() {
        NumberTile tile = activeTile;
        activeTile = null;
        return tile;
    }

    IEnumerator MakeActiveTileFall() {
        if (this.activeTile != null) {
            yield return FallActiveTile();
            NumberTile previousActiveTile = ClearActiveTile();
            List<NumberTile> mergedTiles = MergeNeighbourTiles(previousActiveTile);
            SelectAndDropTiles(previousActiveTile, mergedTiles);

            yield return WaitForBlocksToStopFalling();
            yield return new WaitForSeconds(1);
            GetNewActiveTile();
            ResetFalling();
        }
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

    IEnumerator DropAndMergeTile(NumberTile tile) {
        yield return FallTile(tile);
        List<NumberTile> mergedTiles = MergeNeighbourTiles(tile);
        SelectAndDropTiles(tile, mergedTiles);
    }

    IEnumerator WaitForBlocksToStopFalling() {
        while (fallingTilesQueue.Count != 0) {
            yield return fallingTilesQueue.Dequeue();
        }
    }

    void SelectAndDropTiles(NumberTile selectedTile, List<NumberTile> mergedTiles) {
        if (mergedTiles.Count == 0) {
            return;
        }

        bool dropSelectedTile = true;
        foreach (NumberTile mergedTile in mergedTiles) {
            mergedTile.DeActivate();

            NumberTile neighbour = TryToDropTileAbove(mergedTile);
            if (neighbour != null && selectedTile.IsSameCell(neighbour)) {
                dropSelectedTile = false;
            }
        }

        if (dropSelectedTile) {
            DropTile(selectedTile);
        }
    }

    NumberTile TryToDropTileAbove(NumberTile tile) {
        NumberTile neighbour = tile.FindNeighbourTile(Direction.up);
        if (IsActiveTile(neighbour)) {
            DropTile(neighbour);
            return neighbour;
        }

        return null;
    }

    void DropTile(NumberTile tile) {
        Coroutine fallingCoroutine = StartCoroutine(DropAndMergeTile(tile));
        fallingTilesQueue.Enqueue(fallingCoroutine);
    }

    List<NumberTile> MergeNeighbourTiles(NumberTile tile) {
        List<NumberTile> tiles = new List<NumberTile>();

        foreach (Direction direction in mergeDirections) {
            NumberTile neighbour = tile.FindNeighbourTile(direction);
            if (IsActiveTile(neighbour) && tile.IsEqualNumber(neighbour)) {
                tiles.Add(neighbour);
            }
        }

        int multiplier = (int)Mathf.Pow(2, tiles.Count);
        tile.Multiply(multiplier);
        return tiles;
    }

    NumberTile FindFloorTile(NumberTile tile) {
        NumberTile nextTile = tile.FindNeighbourTile(Direction.down);
        while (IsInactiveTile(nextTile)) {
            tile = nextTile;
            nextTile = tile.FindNeighbourTile(Direction.down);
        }

        return tile;
    }

    bool IsActiveTile(NumberTile tile) {
        return tile != null && tile.Active;
    }

    bool IsInactiveTile(NumberTile tile) {
        return tile != null && !tile.Active;
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
}
