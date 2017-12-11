using UnityEngine;

public struct CellPosition {
    public int x;
    public int y;

    public CellPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public CellPosition NeighbourPosition(Vector2 direction) {
        if (direction == Vector2.up) {
            return new CellPosition(x, y - 1);
        }
        else if (direction == Vector2.right) {
            return new CellPosition(x + 1, y);
        }
        else if (direction == Vector2.down) {
            return new CellPosition(x, y + 1);
        }
        else if (direction == Vector2.left) {
            return new CellPosition(x - 1, y);
        }

        // should throw an error
        return new CellPosition(-1, -1);
    }
}
