public class NumberTile {

    private GameGrid grid;
    private Cell cell;
    private NumberTileView numberTile;

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
        set {
            cell.SetChildActive(value);
        }
        get {
            if (cell != null) {
                return cell.IsChildActive();
            }
            return false;
        }
    }

    public NumberTile FindNeighbourTile(Direction direction) {
        CellPosition neighbourPosition = this.cell.Position.NeighbourPosition(direction);
        if (this.grid.GetCell(neighbourPosition) != null) {
            return new NumberTile(grid, neighbourPosition);
        }

        return null;
    }

    public bool MoveToTile(NumberTile tile) {
        if (!tile.Active) {
            Active = false;
            CopyTileProperties(tile);
            tile.Active = true;
            return true;
        }
        return false;
    }

    void CopyTileProperties(NumberTile tile) {
        this.cell = tile.cell;
        tile.numberTile.Number = this.numberTile.Number;
        this.numberTile = tile.numberTile;
    }
}
