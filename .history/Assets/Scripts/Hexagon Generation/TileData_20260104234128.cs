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
    // public Vector3 worldPosition;

    // public TileData(int x, int z, float yRotation, string biomeName, string prefabName, bool isWater, bool isWall, Vector3 worldPosition)
    // {
    //     this.x = x;
    //     this.z = z;
    //     this.yRotation = yRotation;
    //     this.biomeName = biomeName;
    //     this.prefabName = prefabName;
    //     this.isWater = isWater;
    //     this.isWall = isWall;
    //     this.worldPosition = worldPosition;
    // }
}

[System.Serializable]
public class ObjectData
{
    public int x;
    public int z;
    public int yScale;
    public float yRotation;
    public string objectName;
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
