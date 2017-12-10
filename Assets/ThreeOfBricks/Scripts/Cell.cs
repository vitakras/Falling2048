using UnityEngine;

public class Cell : MonoBehaviour {

    public GameObject tile;

    private GameTile gameTile;
    private bool active;

    // Use this for initialization
    void Start() {
        gameTile = GetComponent<GameTile>();
        Active = false;
    }

    public bool Active {
        get {
            return this.active;
        }
        set {
            this.active = value;
            tile.SetActive(this.active);
        }
    }

    public GameTile GetTile() {
        return gameTile;
    }
}
