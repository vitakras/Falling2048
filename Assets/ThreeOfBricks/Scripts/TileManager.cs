using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public GameObject tilePrefab;
    public GameGrid gameGrid;

	// Use this for initialization
	void Start () {
        CreateTiles();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateTiles() {
        Cell[,] cells = gameGrid.GetAllCells();
        for (int i = 0; i < gameGrid.columns; i++) {
            for (int j = 0; j < gameGrid.rows; j++) {
                GameObject tile = GameObject.Instantiate(tilePrefab);
                cells[j, i].SetChild(tile, false);
            }
        }
    }

    void MoveTile(Vector2 dir) {
        
    }


}
