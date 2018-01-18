using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public CellPosition activeTileSpawnPosition = new CellPosition(2, 0);
    public float tileFallWaitInSeconds = 0.5f;
    public float tileSpawnWaitInSeconds = 1f;
    public float mergeTileDelayInSeconds = 0.5f;
    public RandomTileSelector randomTileSelector;
    public GameGrid gameGrid;

    private NumberTile activeTile;
    private readonly Direction[] mergeDirections = new Direction[] { Direction.left, Direction.right, Direction.down, Direction.up };
    private WaitForSeconds fallWait;
    private WaitForSeconds spawnWait;
    private WaitForSeconds mergeWait;
    private bool inputEnabled = true;
    private IEnumerator fall;
    private Queue<Coroutine> fallingTilesQueue;
    private ITileControllerHandler handler;

    private Queue<NumberTile> fallingTiles;

    void Awake() {
        fallingTilesQueue = new Queue<Coroutine>();
        fallingTiles = new Queue<NumberTile>();

        fallWait = new WaitForSeconds(tileFallWaitInSeconds);
        spawnWait = new WaitForSeconds(tileSpawnWaitInSeconds);
        mergeWait = new WaitForSeconds(mergeTileDelayInSeconds);
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

    public void HandlePlayerInput(Direction direction) {
        if (!inputEnabled || NumberTile.IsInactiveTile(activeTile)) {
            return;
        }

        switch (direction) {
            case Direction.left:
            case Direction.right:
                activeTile.MoveTile(direction);
                break;
            case Direction.down:
                activeTile.DropToFloor();
                DisableControl();
                break;
        }
    }

    void GetNewActiveTile() {
        activeTile = new NumberTile(gameGrid, activeTileSpawnPosition);
        activeTile.Activate();
        activeTile.Number = randomTileSelector.GetRandomTileNumber();
    }

    IEnumerator MakeActiveTileFall() {
        if (this.activeTile != null) {
            yield return FallAndMergeTile(activeTile);
            ResetFallingTile();
        }

        yield return spawnWait;

        if (!IsBoardFull()) {
            GetNewActiveTile();
            StartFallingTile();
            EnableControl();
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
        if (tile.IsOnFloor()) {
            yield break;
        }

        List<NumberTile> aboveTiles = NumberTile.FindActiveTilesInDirection(tile, Direction.up);
        do {
            yield return fallWait;

            tile.MoveTile(Direction.down);
            foreach (NumberTile aboveTile in aboveTiles) {
                aboveTile.MoveTile(Direction.down);
            }
        } while (!tile.IsOnFloor());

        // Add tiles to the queue to process later
        fallingTiles.Enqueue(tile);
        foreach (NumberTile aboveTile in aboveTiles) {
            fallingTiles.Enqueue(aboveTile);
        }
    }

    IEnumerator FallAndMergeTile(NumberTile tile) {
        yield return FallTile(tile);

        if (activeTile != null && tile == activeTile) {
            DisableControl();
        }

        if (fallingTiles.Count != 0) {
            yield return new WaitForSeconds(0.2f);
        }

        List<NumberTile> mergedTiles = new List<NumberTile>();
        while (fallingTiles.Count != 0) {
            NumberTile fallingTile = fallingTiles.Dequeue();
            if (MergeNeighbourTiles(fallingTile, ref mergedTiles)) {
                yield return null;

                // Play and wait for animation to finish
                fallingTile.View.PlayUpdateAnimation();
                yield return new WaitForSeconds(1);

                // Add the Tile again to see if it can merge with anything else
                if (fallingTile.IsOnFloor()) {
                    fallingTiles.Enqueue(fallingTile);
                }
            }
        }

        Queue<Coroutine> fallingTilesQueue = new Queue<Coroutine>();
        foreach (NumberTile mergedTile in mergedTiles) {
            NumberTile neighbour = NumberTile.TryFindActiveTopTile(mergedTile);
            if (NumberTile.IsActiveTile(neighbour)) {
                Coroutine c = StartCoroutine(FallAndMergeTile(neighbour));
                fallingTilesQueue.Enqueue(c);
            }
        }

        while (fallingTilesQueue.Count != 0) {
            yield return fallingTilesQueue.Dequeue();
        }
    }

    bool MergeNeighbourTiles(NumberTile tile, ref List<NumberTile> mergedTiles) {
        int mergeCount = 0;

        foreach (Direction direction in mergeDirections) {
            NumberTile neighbour = tile.FindNeighbourTile(direction);
            if (NumberTile.IsActiveTile(neighbour) && tile.IsEqualNumber(neighbour)) {
                neighbour.DeActivate();
                mergedTiles.Add(neighbour);
                mergeCount++;
            }
        }

        int multiplier = (int)Mathf.Pow(2, mergeCount);
        if (multiplier > 1) {
            tile.Multiply(multiplier);
            if (handler != null) {
                handler.OnTileMerged(tile);
            }

            return true;
        }

        return false;
    }
}

public interface ITileControllerHandler {

    void OnTileMerged(NumberTile tile);

    void OnUnableToCreateNewActiveTile();
}
