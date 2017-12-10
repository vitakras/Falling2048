using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour {

    public float speed;
    private RectTransform rectTransform;

	// Use this for initialization
	void Start () {
        rectTransform = GetComponent<RectTransform>();
	}

    IEnumerator MoveToPosition(RectTransform start, RectTransform end, float speed) {
        float sqrtRemainingDistance = SqrtRemainingDistance(start, end);
        Vector3 newPosition;

        while (sqrtRemainingDistance > float.Epsilon) {
            newPosition = Vector3.MoveTowards(start.anchoredPosition, end.anchoredPosition, speed * Time.deltaTime);
            start.anchoredPosition = newPosition;
            sqrtRemainingDistance = SqrtRemainingDistance(start, end);
            yield return null;
        }

        start.anchoredPosition = end.anchoredPosition;
    }

    private float SqrtRemainingDistance(RectTransform start, RectTransform end) {
        return (start.anchoredPosition - end.anchoredPosition).sqrMagnitude;
    }

}
