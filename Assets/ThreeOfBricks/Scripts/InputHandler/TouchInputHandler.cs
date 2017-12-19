using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputHandler : MonoBehaviour, IDragHandler, IPointerDownHandler {

    public TileManager tilemanager;
    public float offset;
    private Vector2 previousPosition;


    // Use this for initialization
    void Start() {

    }

    public void OnDrag(PointerEventData eventData) {
        if (previousPosition == Vector2.zero) {
            return;
        }

        float horizontal = previousPosition.x - eventData.position.x;
        float vertical = previousPosition.y - eventData.position.y;

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical)) {
            if (offset < Mathf.Abs(horizontal)) {
                previousPosition = eventData.position;
                if (horizontal < 0) {
                    Debug.Log("Move " + Direction.right);
                    tilemanager.HandlePlayerInput(Direction.right);
                }
                else {
                    Debug.Log("Move " + Direction.left);
                    tilemanager.HandlePlayerInput(Direction.left);
                }
               // previousPosition = Vector2.zero;
            }
        }
        else {
            if (offset < Mathf.Abs(vertical)) {
                previousPosition = eventData.position;
                if (vertical > 0) {
                    Debug.Log("Move " + Direction.down);
                    tilemanager.HandlePlayerInput(Direction.down);
                }
                previousPosition = Vector2.zero;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("On Pointer down");
        previousPosition = eventData.position;
    }
}
