using System.Collections.Generic;

public class NumberTile {

    private GameGrid grid;
    private Cell cell;
    private NumberTileView numberTileView;
    private INumberHandler numberHandler;
    private bool hasMoved;

    public NumberTile(GameGrid grid, CellPosition position) {
        this.grid = grid;
        cell = grid.GetCell(position);
        numberTileView = cell.GetChild().GetComponent<NumberTileView>();
    }

    public int Number {
        set {
            numberTileView.Number = value;
        }
        get {
            return numberTileView.Number;
        }
    }

    public bool Active {
        private set {
            cell.SetChildActive(value);
        }
        get {
            if (cell != null) {
                return cell.IsChildActive();
            }
            return false;
        }
    }

    public bool HasMoved {
        get {
            return hasMoved;
        }
    }

    public NumberTileView View {
        get {
            return this.numberTileView;
        }
    }

    public void SetNumberHandler(INumberHandler handler) {
        numberHandler = handler;
    }

    public void ResetMoved() {
        hasMoved = false;
    }

    public NumberTile FindNeighbourTile(Direction direction) {
        CellPosition neighbourPosition = this.cell.Position.NeighbourPosition(direction);
        if (this.grid.GetCell(neighbourPosition) != null) {
            return new NumberTile(grid, neighbourPosition);
        }

        return null;
    }

    public bool MoveToTile(NumberTile tile) {
        if (IsInactiveTile(tile)) {
            this.DeActivate();
            this.CopyTileProperties(tile);
            this.Activate();
            this.hasMoved = true;
            return true;
        }
        return false;
    }

    public bool MoveTile(Direction direction) {
        NumberTile neighbour = FindNeighbourTile(direction);
        return MoveToTile(neighbour);
    }

    public bool DropToFloor() {
        NumberTile floorTile = FindFloorTile(this);
        return MoveToTile(floorTile);
    }

    public bool IsOnFloor() {
        NumberTile neighbour = FindNeighbourTile(Direction.down);
        return neighbour == null || IsActiveTile(neighbour);
    }

    public bool IsEqualNumber(NumberTile tile) {
        return this.Number == tile.Number;
    }

    public bool IsSameCell(NumberTile tile) {
        return this.cell == tile.cell;
    }

    public void Multiply(int multiplier) {
        this.Number = this.Number * multiplier;
    }

    public void DeActivate() {
        this.Active = false;
    }

    public void Activate() {
        this.Active = true;
    }

    void CopyTileProperties(NumberTile tile) {
        this.cell = tile.cell;
        tile.numberTileView.Number = this.numberTileView.Number;
        this.numberTileView = tile.numberTileView;
    }

    public static NumberTile FindFloorTile(NumberTile tile) {
        NumberTile nextTile = tile.FindNeighbourTile(Direction.down);
        while (IsInactiveTile(nextTile)) {
            tile = nextTile;
            nextTile = tile.FindNeighbourTile(Direction.down);
        }

        return tile;
    }

    public static NumberTile TryFindActiveTopTile(NumberTile tile) {
        NumberTile nextTile = tile.FindNeighbourTile(Direction.up);
        while (IsInactiveTile(nextTile)) {
            tile = nextTile;
            nextTile = tile.FindNeighbourTile(Direction.up);
        }

        if (IsActiveTile(nextTile)) {
            return nextTile;
        }


        return null;
    }

    public static List<NumberTile> FindActiveTilesInDirection(NumberTile tile, Direction dir) {
        List<NumberTile> tiles = new List<NumberTile>();

        NumberTile nextTile = tile.FindNeighbourTile(dir);
        while (IsActiveTile(nextTile)) {
            tiles.Add(nextTile);
            nextTile = nextTile.FindNeighbourTile(dir);
        }

        return tiles;
    }

    public static bool IsInactiveTile(NumberTile tile) {
        return tile != null && !tile.Active;
    }

    public static bool IsActiveTile(NumberTile tile) {
        return tile != null && tile.Active;
    }
}

public interface INumberHandler {

    void OnTileMoved(NumberTile tile, Direction direction);

}
