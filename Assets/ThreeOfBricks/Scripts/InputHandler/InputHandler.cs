using UnityEngine;

public class InputHandler : MonoBehaviour {

    public Direction GetDirection() {
        if (Input.GetKeyDown(KeyCode.A)) {
            return Direction.left;
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            return Direction.right;
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            return Direction.down;
        }

        return Direction.none;
    }
}
