using UnityEngine;

/// <summary>
/// Class For Managing To Show only one Visible active panel
/// </summary>
public class PanelSelector : MonoBehaviour {

    public int defaultPanel = 0;
    public Transform[] panels;

    private int activePanel = -1;

    // Use this for initialization
    void Start() {
        this.activePanel = 0;
        DeActiveAllPanels();
        this.SetActivePanel(this.defaultPanel);
    }

    public void SetActivePanel(int index) {
        this.panels[this.activePanel].gameObject.SetActive(false);
        this.activePanel = index;
        this.panels[this.activePanel].gameObject.SetActive(true);
    }

    public void DeActiveAllPanels() {
        foreach (Transform transform in panels) {
            transform.gameObject.SetActive(false);
        }
    }
}
