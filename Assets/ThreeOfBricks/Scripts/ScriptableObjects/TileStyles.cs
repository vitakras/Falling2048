using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class TileStyle {
    public int number;
    public Color tileColor;
    public Color textColor;
}

[CreateAssetMenu(fileName = "tilestyles", menuName = "ThreeOfBlocks/TileStyles", order = 1)]
public class TileStyles : ScriptableObject {

    public TileStyle[] tilesStyleList;
    public TileStyle defaultStyle;
    public Sprite background;

    private Dictionary<int, TileStyle> styles;

    public void Awake() {
        Initialize();
    }

    public void ApplyStyle(NumberTileView tile) {
        if (styles == null) {
            Initialize();
            Debug.Log("Style Initialized");
        }

        TileStyle style = defaultStyle;
        if (!styles.TryGetValue(tile.number, out style)) {
            style = defaultStyle;
        }

        ApplyTileStyle(style, tile);
    }

    private void ApplyTileStyle(TileStyle style, NumberTileView tile) {
        tile.SetBackground(background);
        tile.SetBackgroundColor(style.tileColor);
        tile.SetTextColor(style.textColor);
    }

    private void Initialize() {
        styles = new Dictionary<int, TileStyle>();
        foreach (TileStyle style in tilesStyleList) {
            styles.Add(style.number, style);
        }
    }
}
