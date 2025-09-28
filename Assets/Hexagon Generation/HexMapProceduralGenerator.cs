using UnityEngine;

public class HexMapProceduralGenerator : MonoBehaviour
{
    [System.Serializable]
    public class BiomeType
    {
        public string name;             // Name des Bioms
        public GameObject[] mapTiles;   // Array der Map Tiles für dieses Biom
        public float percentage;        // Prozentuale Häufigkeit dieses Bioms
        public bool isWater;            // Gibt an, ob es sich um ein Wasser-Biom handelt
    }

    [Tooltip("Your biomes. Contains the name, mapTiles and the weight.")]
    public BiomeType[] biomes;          // Array der verschiedenen Biome
    [Tooltip("Map width size")]
    public int mapWidth = 10;           // Breite der Karte
    [Tooltip("Map height size")]
    public int mapHeight = 10;          // Höhe der Karte
    [Tooltip("Perlin noise scale")]
    public float scale = 1.0f;          // Skala für das Perlin-Rauschen
    private float xOffsetRange = 200f;  // Bereich für das zufällige x-Offset
    private float yOffsetRange = 200f;  // Bereich für das zufällige y-Offset
    [Tooltip("Ocean width relative to map size")]
    public float oceanWidth = 0.3f;     // Breite des Ozeans relativ zur Kartengröße
    [Tooltip("Spacing between tiles")]
    public float tileSpacing = 1.0f;    // Tile spacing for square tile


    void Start()
    {
        Random.InitState(System.Environment.TickCount);
        GenerateMap();
    }
    public void ClearMap()
    {
        Transform oldHolder = transform.Find("MapHolder");
        if (oldHolder != null)
        {
            DestroyImmediate(oldHolder.gameObject);
        }
    }

    public void GenerateMap()
    {
        ClearMap();

        GameObject holderObject = new GameObject("MapHolder"); // Erstelle ein neues GameObject als Holder
        holderObject.transform.parent = transform; // Mache das Holder-Objekt zum Kind des TerrainGenerator-Objekts

        float xOffset = Random.Range(-xOffsetRange, xOffsetRange);
        float yOffset = Random.Range(-yOffsetRange, yOffsetRange);

        float[,] falloffMap = GenerateFalloffMap(mapWidth, mapHeight);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float xCoord = ((float)x / mapWidth + xOffset) * scale;
                float yCoord = ((float)y / mapHeight + yOffset) * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord) - falloffMap[x, y];

                GameObject mapTile = DetermineMapTile(sample);

                // Vector3 position = new Vector3(x * tileSpacing, 0, y * tileSpacing); // For square tile

                // === Flat-top hex placement ===
                float hexWidth = 2f;      // prefab X size
                float hexHeight = 2.309f; // prefab Z size

                float posX = x * hexWidth + (y % 2 == 1 ? hexWidth / 2f : 0f);
                float posZ = y * (hexHeight * 0.75f);

                Vector3 position = new Vector3(posX, 0, posZ);

                GameObject tileInstance = Instantiate(mapTile, position, Quaternion.identity);
                tileInstance.transform.parent = holderObject.transform; // Make the tile a child of the holder object

                /*// Set Tag base on chosen Biome
                foreach (BiomeType biome in biomes)
                {
                    float randomValue = Random.Range(0f, 100f);
                    if (randomValue <= biome.percentage)
                    {
                        tileInstance.tag = biome.name;
                        break;
                    }
                }*/
            }
        }
    }

    GameObject DetermineMapTile(float sample)
    {
        // Determine the biome based on the Perlin noise value
        if (sample <= 0) // If the sample value is less than or equal to zero, use the water biome 
        {
            return GetWaterTile();
        }

        float cumulativePercentage = 0f;
        foreach (BiomeType biome in biomes)
        {
            cumulativePercentage += biome.percentage;
            if (sample * 100f <= cumulativePercentage)
            {
                // Randomly select a map tile from this biome's array  
                int index = Random.Range(0, biome.mapTiles.Length);
                return biome.mapTiles[index];
            }
        }

        // Default: Return the last biome if no biome was selected  
        BiomeType defaultBiome = biomes[biomes.Length - 1];
        int defaultIndex = Random.Range(0, defaultBiome.mapTiles.Length);
        return defaultBiome.mapTiles[defaultIndex];
    }

    GameObject GetWaterTile()
    {
        // Randomly select a water tile from the biomes that are marked as water
        foreach (BiomeType biome in biomes)
        {
            if (biome.isWater)
            {
                int index = Random.Range(0, biome.mapTiles.Length);
                return biome.mapTiles[index];
            }
        }
        return null; // Fallback in case no water biome is found
    }

    float[,] GenerateFalloffMap(int width, int height)
    {
        float[,] map = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        // Use oceanWidth to adjust the transition to the water zone
        return Mathf.Pow(value * oceanWidth, a) / (Mathf.Pow(value * oceanWidth, a) + Mathf.Pow(b - b * value * oceanWidth, a));
    }
}
