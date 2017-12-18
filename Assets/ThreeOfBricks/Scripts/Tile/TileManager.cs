using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour, INumberHandler {

    public CellPosition activeTileSpawnPosition = new CellPosition(2, 0);
    public float tileFallWaitInSeconds = 0.5f;
    public float tileSpawnWaitInSeconds = 1f;
    public RandomTileSelector randomTileSelector;
    public GameGrid gameGrid;
    public InputHandler inputHandler;

    private NumberTile activeTile;
    private readonly Direction[] mergeDirections = new Direction[] { Direction.left, Direction.right, Direction.down };
    private WaitForSeconds fallWait;
    private WaitForSeconds spawnWait;
    private bool inputEnabled = true;
    private IEnumerator fall;
    private Queue<Coroutine> fallingTilesQueue;
    private ITileControllerHandler handler;

    void Awake() {
        fallingTilesQueue = new Queue<Coroutine>();
        fallWait = new WaitForSeconds(tileFallWaitInSeconds);
        spawnWait = new WaitForSeconds(tileSpawnWaitInSeconds);
    }

    // Update is called once per frame
    void Update() {
        if (!inputEnabled || NumberTile.IsInactiveTile(activeTile)) {
            return;
        }
        HandlePlayerInput();
    }

    public ITileControllerHandler Handler {
        get {
            return handler;
        }

        set {
            handler = value;
        }
    }

    public void EnableControl() {
        this.inputEnabled = true;
    }

    public void DisableControl() {
        this.inputEnabled = false;
    }

    public void StartFallingTile() {
        fall = MakeActiveTileFall();
        StartCoroutine(fall);
    }

    public void StopFallingTile() {
        if (fall != null) {
            StopCoroutine(fall);
        }
    }

    public void ResetFallingTile() {
        activeTile = null;
    }

    public void OnTileMoved(NumberTile tile, Direction direction) {
        if (direction == Direction.down) {
            NumberTile previousPositionTile = tile.FindNeighbourTile(Direction.up);
            TryToDropActiveTileAbove(previousPositionTile);
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

    void GetNewActiveTile() {
        activeTile = new NumberTile(gameGrid, activeTileSpawnPosition);
        activeTile.Activate();
        activeTile.Number = randomTileSelector.GetRandomTileNumber();
        activeTile.SetNumberHandler(this);
    }

    IEnumerator MakeActiveTileFall() {
        if (this.activeTile != null) {
            yield return FallTile(activeTile);
            ResetFallingTile();
        }

        yield return WaitForBlocksToStopFalling();
        yield return spawnWait;

        if (!IsBoardFull()) {
            GetNewActiveTile();
            StartFallingTile();
        }
        else {
            Debug.Log("board is full");
            if (handler != null) {
                handler.OnUnableToCreateNewActiveTile();
            }
        }
    }

    bool IsBoardFull() {
        return (new NumberTile(gameGrid, activeTileSpawnPosition).Active);
    }

    IEnumerator FallTile(NumberTile tile) {
        Debug.Log("Tile Started Falling");
        while (!tile.IsOnFloor()) {
            yield return fallWait;
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

            NumberTile neighbour = TryToDropActiveTileAbove(mergedTile);
            if (neighbour != null && selectedTile.IsSameCell(neighbour)) {
                dropSelectedTile = false;
            }
        }

        if (dropSelectedTile) {
            DropTile(selectedTile);
        }
    }

    NumberTile TryToDropActiveTileAbove(NumberTile tile) {
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

        int multiplier = (int)Mathf.Pow(2, tiles.Count);
        if (multiplier > 1) {
            tile.Multiply(multiplier);
            if (handler != null) {
                handler.OnTileMerged(tile);
            }
        }

        return tiles;
    }
}

public interface ITileControllerHandler {

    void OnTileMerged(NumberTile tile);

    void OnUnableToCreateNewActiveTile();
}
