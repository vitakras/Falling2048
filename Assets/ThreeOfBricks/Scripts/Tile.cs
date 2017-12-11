using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    private GameGrid grid;
    private Cell cell;
    private TileView tileView;

    public Tile(GameGrid grid, CellPosition position) {
        this.grid = grid;
        UpdateCell(position);
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

    public Tile FindNeighbourTile(Vector2 direction) {
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
        this.tileView = tile.tileView;
    }

    Cell FindNeighbourCell(Vector2 direction) {
        CellPosition neighbourPosition = this.cell.Position.NeighbourPosition(direction);
        return grid.GetCell(neighbourPosition);
    }

    bool CanMoveToCell(Cell cell) {
        return !cell.IsChildActive();
    }

    void UpdateCell(CellPosition position) {
        cell = grid.GetCell(position);
    }
}
