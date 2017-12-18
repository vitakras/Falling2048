using UnityEngine;

public class InputHandler : MonoBehaviour {

    public TileManager tilemanager;

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            tilemanager.HandlePlayerInput(Direction.left);
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            tilemanager.HandlePlayerInput(Direction.right);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            tilemanager.HandlePlayerInput(Direction.down);
        }
    }
}
