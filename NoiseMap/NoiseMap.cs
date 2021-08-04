using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

public class NoiseMap : MonoBehaviour
{
    public Biochemical[] bios;

    public int MapW = 50, MapH = 50;
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
    public Tile tile;
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
        // height map
        heightMap = NoiseGenerate.Generate(MapW, MapH, scale, offset, heightWaves);
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
                //Vector3Int[] poss = K_utility.GetRoundPos(new Vector3Int(x, y, 1));
                if (x == 9 && y == 43)
                {
                    print("here");

                }
                Biochemical bim = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], x, y);
                if (bim.tile == null)
                {
                    Sprite spr = bim.GetTleSprite();
                    tile.sprite = spr;
                    tileMap.SetTile(tilepos, tile);
                }
                else
                {
                      tileMap.SetTile(tilepos, bim.tile);
                }




            }
        }
    }

    public void OrderedTile()
    {

    }

    [Button("CleanMap")]
    void CleanMap()
    {
        tileMap.ClearAllTiles();
    }

    //���������ĸ�Ⱥ��
    Biochemical GetBiome(float height, float moisture, float heat, int x, int y)
    {

        float MinDiff = float.MaxValue;
        Biochemical MinBio = null;
        List<BiomeData> biomeDatas = new List<BiomeData>();
        Debug.Log(x + "," + y + "::" + height + "," + moisture + "," + heat);
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
        print("û������Ⱥ��");
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
