using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputHandler : MonoBehaviour, IDragHandler, IPointerDownHandler {

    public float offset;
    private Vector2 previousPosition;


    // Use this for initialization
    void Start() {

    }

    public void OnDrag(PointerEventData eventData) {
        float difference = previousPosition.x - eventData.position.x;
        if (offset < Mathf.Abs(difference)) {
            previousPosition = eventData.position;
            if (difference < 0) {
                Debug.Log("Move " + Direction.right);
            } else {
                Debug.Log("Move " + Direction.left);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("On Pointer down");
        previousPosition = eventData.position;
    }
}
