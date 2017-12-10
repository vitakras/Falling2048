using UnityEngine;

public class Cell : MonoBehaviour {

    public GameObject child;
    private CellPosition position;

    public void SetChild(GameObject child, bool active = false) {
        this.child = child;
        this.child.transform.SetParent(this.transform, false);
        SetChildActive(active);
    }

    public void SetChildActive(bool active) {
        this.child.SetActive(active);
    }

    public bool IsChildActive() {
        if (child != null) {
            return child.activeSelf;
        }

        return false;
    }

    public CellPosition Position {
        set {
            position = value;
        }
        get {
            return position;
        }
    }
}
