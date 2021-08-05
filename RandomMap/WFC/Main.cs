using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MyWfcTest;
using UnityEngine.Tilemaps;
using System.Linq;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    BoundsInt bounds;
    Modle[,,] modles;
    Dictionary<TileBase, ModelRulesInfor> CurTileRule;
    Tilemap[] CurTileMap;
    Stack<Vector3Int> MinPoss = new Stack<Vector3Int>();
     static Main instance;
     public static Main Instance{
         get{
             if(instance == null){
                 instance = new Main();
             }
             return instance;
         }
     }
    private void Awake() {
        instance = this;
    }


    void InitMap(Dictionary<TileBase, ModelRulesInfor> TileRules,BoundsInt bouns)
    {
        this.bounds.size = bouns.size;
        CurTileRule = TileRules;
        modles = new Modle[bounds.size.x, bounds.size.y, bounds.size.z];
        for (int x = 0; x < bounds.size.x; x++)
            for (int y = 0; y < bounds.size.y; y++)
                for (int z = 0; z < bounds.size.z; z++)
                {

                    modles[x, y, z] = new Modle(CurTileRule.Keys, 6,TileRules);
                }
    }
 

    public void GeneratorWFCMap(Dictionary<TileBase, ModelRulesInfor> TileRules,BoundsInt boundsInt,Tilemap[] tilemap)
    {
        CurTileMap = tilemap;
        MinPoss.Clear();
        InitMap(TileRules,boundsInt);
        for (int i = 0; i < tilemap.Length; i++)
        {
            tilemap[i].ClearAllTiles();
        }
        
        StartCoroutine(StartWave());
    }

     IEnumerator StartWave()
    {
        while (observe(out var MinTilePos))
        {
            StartCoroutine(propagate());
            if(MinTilePos.z==0){
                print("sss"+MinTilePos.z);
            }
            Modle modle = modles[MinTilePos.x, MinTilePos.y, MinTilePos.z];
            Vector3Int TileMapPos = CurTileMap[MinTilePos.z].WorldToCell(bounds.min + Vector3Int.one + MinTilePos);
            CurTileMap[MinTilePos.z].SetTile(TileMapPos, modle.CurTile);
            
        }
        yield return null;
    }

   
     bool observe(out Vector3Int MinTilePos)
    {
        var minEntrop = float.MaxValue;
        var minPos = -Vector3Int.one;
        for (int x = 0; x < bounds.size.x; x++)
            for (int y = 0; y < bounds.size.y; y++)
                for (int z = 0; z < bounds.size.z; z++)
                {
                    Modle modle = modles[x, y, z];
                    if (modle.Confirm)
                    {
                        continue;
                    }
                    float bias = 0.1f + Random.Range(0, 0.5f);
                    if (modle.Entrop + bias < minEntrop)
                    {
                        minEntrop = modle.Entrop + bias;
                        minPos = new Vector3Int(x, y, z);
                    }
                }
        MinTilePos = default;
        if (minPos.x < 0)
            return false;
        Modle modle2 = modles[minPos.x, minPos.y, minPos.z];
        TileBase tile = GetWeightRandom(modle2.SuperPosition);
        modle2.CollapseTo(tile);
        modles[minPos.x, minPos.y, minPos.z] = modle2;
        MinPoss.Push(minPos);
        MinTilePos = minPos;
        // pattern.GetMapText(minPos,Color.red,modle2.SuperPosition.Count.ToString());
        return true;
    }

    /// <summary>
    /// 根据权重随机
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
     TileBase GetWeightRandom(IEnumerable<TileBase> tiles)
    {
        float WeightSum = tiles.Sum(tile => CurTileRule[tile].TileWeight);
        float r = Random.Range(0, WeightSum);
        float curWight = 0;
        foreach (var item in tiles)
        {
            float Weight = CurTileRule[item].TileWeight;
            curWight += Weight;
            if (curWight >= r)
            {
                return item;
            }
        }
        return null;
    }
     IEnumerator propagate()
    {
        while (MinPoss.Count > 0)
        {
            var ChangedPos = MinPoss.Pop();
            var modle = modles[ChangedPos.x, ChangedPos.y, ChangedPos.z];
            Vector3Int[] AdjacentPos = K_utility.GetSixRoundPos(ChangedPos);
            for (int i = 0; i < AdjacentPos.Length; i++)
            {
                if (!bounds.Contains(AdjacentPos[i]))
                {
                    continue;
                }
                var adjacentModel = modles[AdjacentPos[i].x, AdjacentPos[i].y, AdjacentPos[i].z];

                if (adjacentModel.IsUpdateNeighborFrom(modle.NeighborOffset[i]))
                {
                    if (adjacentModel.SuperPosition.Count <= 0)
                    {
                        
                        break;
                       
                    }
                    MinPoss.Push(AdjacentPos[i]);
                    modles[AdjacentPos[i].x, AdjacentPos[i].y, AdjacentPos[i].z] = adjacentModel;//给Models赋值

                }
            }
            
        }
        yield return null;

    }

}
