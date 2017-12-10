using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    private GameGrid grid;
    private CellPosition position;
    private TileView tile;

    public Tile(GameGrid grid, CellPosition position) {
        this.grid = grid;
        this.position = position;
    }

    void ActivateTile() {
        Cell cell = grid.GetCell(position);
        cell.SetChildActive(true);
    }

    Cell FindNeighbourCell(Vector2 direction) {
        CellPosition neighbourPosition = this.position.NeighbourPosition(direction);
        return grid.GetCell(neighbourPosition);
    }
}
