using UnityEngine;

public class GameGrid : MonoBehaviour {

    public GameObject cellPrefab;
    public int rows;
    public int columns;

    private Cell[,] cells;

    void Awake() {
        PopulateGrid();
    }

    public Cell[,] GetAllCells() {
        return cells;
    }

    public Cell GetCell(CellPosition position) {
        if (CellPositionExistsOnGrid(position)) {
            return cells[position.x, position.y];
        }

        return null;
    }

    void PopulateGrid() {
        cells = new Cell[rows, columns];

        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                cells[j, i] = CreateNewCell(j, i);
            }
        }
    }

    Cell CreateNewCell(int x, int y) {
        GameObject newCell = GameObject.Instantiate(cellPrefab);
        newCell.transform.SetParent(this.transform, false);

        Cell cell = newCell.GetComponent<Cell>();
        cell.Position = new CellPosition(x, y);
        return cell;
    }

    bool CellPositionExistsOnGrid(CellPosition position) {
        if (position.x >= 0 && position.y >= 0 &&
            position.x < rows && position.y < columns) {
            return true;
        }

        return false;
    }
}
