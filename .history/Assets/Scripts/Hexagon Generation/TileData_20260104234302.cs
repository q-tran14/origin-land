using UnityEngine;

[System.Serializable]
public class TileData
{
    public int x;
    public int z;
    public float yRotation;
    public string biomeName;
    public string prefabName;
    public bool isWater;
    public bool isWall;

    [System.NonSerialized] public GameObject tileGO; // runtime only
    public float height; // dùng cho save/load
}

[System.Serializable]
public class ObjectData
{
    public int tileX;
    public int tileZ;
    public string prefabName;
    public float yOffset;
    public float yRotation;
}

[System.Serializable]
public class MapSaveData
{
    public int width;
    public int height;
    public int seed;
    public string mapName;
    public TileData[] tiles;
    public ObjectData[] objects;
}
