using UnityEngine;

[System.Serializable]
public class TileData {
    public int x;
    public int y;
    public string biomeName;
    public bool isWater;
    public int waterEdges;
    public bool isCorner;
}

[System.Serializable]
public class MapSaveData {
    public int width;
    public int height;
    public int seed;
    public TileData[] tiles;
}
