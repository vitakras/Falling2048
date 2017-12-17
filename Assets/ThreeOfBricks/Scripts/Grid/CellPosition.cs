[System.Serializable]
public struct CellPosition {
    public int x;
    public int y;

    public CellPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public CellPosition NeighbourPosition(Direction direction) {
        if (direction == Direction.up) {
            return new CellPosition(x, y - 1);
        }
        else if (direction == Direction.right) {
            return new CellPosition(x + 1, y);
        }
        else if (direction == Direction.down) {
            return new CellPosition(x, y + 1);
        }
        else if (direction == Direction.left) {
            return new CellPosition(x - 1, y);
        }

        return new CellPosition(-1, -1);
    }
}
