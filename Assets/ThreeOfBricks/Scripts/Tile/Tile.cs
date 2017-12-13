using UnityEngine;

public class Tile {

    private GameGrid grid;
    private Cell cell;
    private NumberTile numberTile;

    public Tile(GameGrid grid, CellPosition position) {
        this.grid = grid;
        UpdateCell(position);
    }

    public void SetNumber(int number) {
        numberTile.Number = number;
    }

    public int GetNumber() {
        return numberTile.Number;
    }

    public bool ActiveTile {
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

    public Tile FindNeighbourTile(Direction direction) {
        CellPosition neighbourPosition = this.cell.Position.NeighbourPosition(direction);
        if (this.grid.GetCell(neighbourPosition) != null) {
            return new Tile(grid, neighbourPosition);
        }

        return null;
    }

    public bool MoveToTile(Tile tile) {
        if (!tile.ActiveTile) {
            ActiveTile = false;
            Copy(tile);
            ActiveTile = true;
            return true;
        }
        return false;
    }

    private void Copy(Tile tile) {
        this.cell = tile.cell;
        tile.numberTile.Number = this.numberTile.Number;
        this.numberTile = tile.numberTile;
    }

    Cell FindNeighbourCell(Direction direction) {
        CellPosition neighbourPosition = this.cell.Position.NeighbourPosition(direction);
        return grid.GetCell(neighbourPosition);
    }

    bool CanMoveToCell(Cell cell) {
        return !cell.IsChildActive();
    }

    void UpdateCell(CellPosition position) {
        cell = grid.GetCell(position);
        numberTile = cell.GetChild().GetComponent<NumberTile>();
    }
}
