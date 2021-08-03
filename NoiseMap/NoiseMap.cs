using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

public class NoiseMap : MonoBehaviour
{
    public Biochemical[] bios;

    public int MapW = 50,MapH = 50;
    public float scale = 1f;
    public Vector2 offset;
    public Tilemap tileMap;

    [Header("Height Map")]
    public Wave[] heightWaves;
    public float[,] heightMap;
    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    private float[,] moistureMap;
    [Header("Heat Map")]
    public Wave[] heatWaves;
    private float[,] heatMap;
    Tile tile;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        GenerateMap();
    }

    [Button("GenerateMap")]
    void GenerateMap()
    {
        tileMap.ClearAllTiles();
        tile = (Tile)Resources.Load<TileBase>("Tile");
        // height map
        heightMap = NoiseGenerate.Generate(MapW, MapH, scale,offset,heightWaves);
        // moisture map
        moistureMap = NoiseGenerate.Generate(MapW, MapH, scale, offset, moistureWaves);
        // heat map
        heatMap = NoiseGenerate.Generate(MapW, MapH, scale, offset, heatWaves);
        for (int x = 0; x < MapW; ++x)
        {
            for (int y = 0; y < MapH; ++y)
            {
                // GameObject tile = Instantiate(tilePre, new Vector3(x, y, 0), Quaternion.identity,transform);
                Vector3Int tilepos = tileMap.WorldToCell(new Vector3Int(x, y, 1));
                
                Sprite spr = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]).GetTleSprite();
                tile.sprite = spr;
                tileMap.SetTile(tilepos, tile);
                
            }
        }
    }

    [Button("CleanMap")]
    void CleanMap()
    {
        tileMap.ClearAllTiles();
    }

    //���������ĸ�Ⱥ��
    Biochemical GetBiome(float height, float moisture, float heat)
    {
        float MinDiff = float.MaxValue;
        Biochemical MinBio = null;
        List<BiomeData> biomeDatas = new List<BiomeData>();
        Debug.Log(height + "," + moisture + "," + heat);
        foreach (var item in bios)
        {
            if (item.MatchCondition(height, moisture, heat))
            {
                biomeDatas.Add(new BiomeData(item));
            }
        }
        foreach (var item in biomeDatas)
        {
            float CurDiff = item.GetDiffValue(height, moisture, heat);
            if (CurDiff < MinDiff)
            {
                MinDiff = CurDiff;
                MinBio = item.bio;
            }
        }
        if (MinBio != null)
        {
            return MinBio;
        }
        return null;
    }
}

public class BiomeData
{
    public Biochemical bio;
    public BiomeData(Biochemical Bio)
    {
        bio = Bio;
    }

    /// <summary>
    /// Ⱥ�����뵱ǰ�������ڵ���С����ֵ
    /// </summary>
    /// <param name="height"></param>
    /// <param name="moisture"></param>
    /// <param name="heat"></param>
    /// <returns></returns>
    public float GetDiffValue(float height, float moisture, float heat)
    {
        return (height - bio.minHeight) + (moisture - bio.minMoisture) + (heat - bio.minHeat);
    }

}
