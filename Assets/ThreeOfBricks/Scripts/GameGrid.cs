using UnityEngine;

public class GameGrid : MonoBehaviour {

    public GameObject cell;
    public int rows;
    public int columns;

    private Cell[,] cells;

    // Use this for initialization
    void Start() {
        PopulateGrid();
    }

    public Cell GetCell(int row, int column) {
        return cells[row, column];
    }

    public void Reset() {
        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                cells[j, i].Active = false;
            }
        }
    }

    void PopulateGrid() {
        cells = new Cell[rows, columns];

        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                GameObject newCell = GameObject.Instantiate(cell);
                newCell.transform.SetParent(this.transform, false);
                cells[j, i] = newCell.GetComponent<Cell>();
            }
        }
    }
}
