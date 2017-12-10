using UnityEngine;

public class GameGrid : MonoBehaviour {

    public GameObject cellPrefab;
    public int rows;
    public int columns;

    private Cell[,] cells;

    // Use this for initialization
    void Awake() {
        PopulateGrid();
    }

    public Cell[,] GetAllCells() {
        return cells;
    }

    public Cell GetCell(int row, int column) {
        return cells[row, column];
    }

    public void Reset() {

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
}
