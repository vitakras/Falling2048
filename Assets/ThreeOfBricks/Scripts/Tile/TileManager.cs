using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, INumberUpdateHandler, INumberHandler {

    public RandomTileSelector randomTileSelector;
    public Score score;
    public CellPosition activeTileSpawnPosition = new CellPosition(2, 0);
    public GameObject numberedTilePrefab;
    public GameGrid gameGrid;
    public TileStyles style;
    public InputHandler inputHandler;

    private NumberTile activeTile;
    private Coroutine fall;
    private bool inputEnabled = true;
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
        if (!inputEnabled || NumberTile.IsInactiveTile(activeTile)) {
            return;
        }
        HandlePlayerInput();
    }

    public void OnNumberUpdated(NumberTileView numberTile) {
        this.style.ApplyStyle(numberTile);
    }

    public void OnTileMoved(NumberTile tile, Direction direction) {
        if (direction == Direction.down) {
            NumberTile previousPositionTile = tile.FindNeighbourTile(Direction.up);
            TryToDropTileAbove(previousPositionTile);
        }
    }

    public void OnTileHitFloor(NumberTile tile) {
        List<NumberTile> mergedTiles = MergeNeighbourTiles(tile);
        SelectAndDropTiles(tile, mergedTiles);
    }

    void HandlePlayerInput() {
        Direction direction = inputHandler.GetDirection();
        switch (direction) {
            case Direction.left:
            case Direction.right:
                activeTile.MoveTile(direction);
                break;
            case Direction.down:
                activeTile.DropToFloor();
                break;
        }
    }

    void ResetFalling() {
        if (fall != null) {
            StopCoroutine(fall);
        }

        fall = StartCoroutine(MakeActiveTileFall());
    }

    void GetNewActiveTile() {
        activeTile = new NumberTile(gameGrid, activeTileSpawnPosition);
        activeTile.Activate();
        activeTile.Number = randomTileSelector.GetRandomTileNumber();
        activeTile.SetNumberHandler(this);
    }

    NumberTile ClearActiveTile() {
        NumberTile tile = activeTile;
        activeTile = null;
        return tile;
    }

    IEnumerator MakeActiveTileFall() {
        if (this.activeTile != null) {
            yield return FallTile(activeTile);
            ClearActiveTile();
            yield return WaitForBlocksToStopFalling();
            yield return new WaitForSeconds(1);
            if (!IsBoardFull()) {
                GetNewActiveTile();
                ResetFalling();
            } else {
                Debug.Log("board is full");
            }
        }
    }

    private bool IsBoardFull() {
        return (new NumberTile(gameGrid, activeTileSpawnPosition).Active);
    }

    IEnumerator FallTile(NumberTile tile) {
        Debug.Log("Tile Started Falling");
        while (!tile.IsOnFloor()) {
            yield return new WaitForSeconds(0.5f);
            if (!tile.HasMoved) {
                tile.MoveTile(Direction.down);
            }
            tile.ResetMoved();
        }
        Debug.Log("Tile Finished Falling");
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
        if (NumberTile.IsActiveTile(neighbour)) {
            DropTile(neighbour);
            return neighbour;
        }

        return null;
    }

    void DropTile(NumberTile tile) {
        tile.SetNumberHandler(this);
        Coroutine fallingCoroutine = StartCoroutine(FallTile(tile));
        fallingTilesQueue.Enqueue(fallingCoroutine);
    }

    List<NumberTile> MergeNeighbourTiles(NumberTile tile) {
        List<NumberTile> tiles = new List<NumberTile>();

        foreach (Direction direction in mergeDirections) {
            NumberTile neighbour = tile.FindNeighbourTile(direction);
            if (NumberTile.IsActiveTile(neighbour) && tile.IsEqualNumber(neighbour)) {
                tiles.Add(neighbour);
            }
        }

        int previousNumber = tile.Number;
        int multiplier = (int)Mathf.Pow(2, tiles.Count);
        tile.Multiply(multiplier);

        if (previousNumber != tile.Number) {
            this.score.IncreaseScore(tile.Number);
        }
        return tiles;
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
