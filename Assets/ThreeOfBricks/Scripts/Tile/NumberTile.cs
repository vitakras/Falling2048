public class NumberTile {

    private GameGrid grid;
    private Cell cell;
    private NumberTileView numberTile;
    private INumberHandler numberHandler;

    public NumberTile(GameGrid grid, CellPosition position) {
        this.grid = grid;
        cell = grid.GetCell(position);
        numberTile = cell.GetChild().GetComponent<NumberTileView>();
    }

    public int Number {
        set {
            numberTile.Number = value;
        }
        get {
            return numberTile.Number;
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

    public void SetNumberHandler(INumberHandler handler) {
        numberHandler = handler;
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
            CopyTileProperties(tile);
            tile.Activate();
            return true;
        }
        return false;
    }

    public bool MoveTile(Direction direction) {
        NumberTile neighbour = FindNeighbourTile(direction);
        return MoveToTile(neighbour);
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
        tile.numberTile.Number = this.numberTile.Number;
        this.numberTile = tile.numberTile;
    }

    NumberTile FindFloorTile(NumberTile tile) {
        NumberTile nextTile = tile.FindNeighbourTile(Direction.down);
        while (IsInactiveTile(nextTile)) {
            tile = nextTile;
            nextTile = tile.FindNeighbourTile(Direction.down);
        }

        return tile;
    }

    public static bool IsInactiveTile(NumberTile tile) {
        return tile != null && !tile.Active;
    }

    public bool IsActiveTile(NumberTile tile) {
        return tile != null && tile.Active;
    }
}

public interface INumberHandler {

}
