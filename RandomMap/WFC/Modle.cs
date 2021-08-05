using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using MyWfcTest;

public class Modle
{
    Dictionary<TileBase, ModelRulesInfor> GetTileRule;
    public HashSet<TileBase> SuperPosition;
    public HashSet<TileBase>[] NeighborOffset;
    public bool Confirm = false;

    public TileBase CurTile;
    public float Entrop = 0;

    public Modle(IEnumerable<TileBase> AllPossible, int NeighborCount, Dictionary<TileBase, ModelRulesInfor> TileRules)
    {
       // Debug.Log(AllPossible);
        GetTileRule = TileRules;
        SuperPosition = new HashSet<TileBase>(AllPossible);
        NeighborOffset = new HashSet<TileBase>[NeighborCount];
        for (var i = 0; i < NeighborCount; i++)
                NeighborOffset[i] = new HashSet<TileBase>();

        UpdateEntrop();
        UpdateAdjacency();
    }

    /// <summary>
    /// 是否需要更新旁边的Model
    /// </summary>
    /// <param name="FatheModelAdjacency">传递过来的Model邻居信息</param>
    /// <returns></returns>
    public bool IsUpdateNeighborFrom(IEnumerable<TileBase> FatheModelAdjacency)
    {
        if (!SuperPosition.IsSubsetOf(FatheModelAdjacency))
        {
            SuperPosition.IntersectWith(FatheModelAdjacency);
            UpdateAdjacency();
            UpdateEntrop();
            return true;
        }
        return false;
    }


    /// <summary>
    /// 更新邻居的瓷砖
    /// </summary>
    public void UpdateAdjacency()
    {
        for (int i = 0; i < NeighborOffset.Length; i++)
        {
            NeighborOffset[i].Clear();
            foreach (var item in SuperPosition)
            {
                NeighborOffset[i].UnionWith(GetTileRule[item].Dic_DirTiles[i]);
            }
            
        }
    }

    /// <summary>
    /// 强制坍缩成确定瓷砖
    /// </summary>
    /// <param name="tile"></param>
    public void CollapseTo(TileBase tile)
    {
        SuperPosition.Clear();
        SuperPosition.Add(tile);
        Entrop = 0;
        Confirm = true;
        CurTile = tile;
        UpdateAdjacency();
    }


    /// <summary>
    /// 更新熵
    /// </summary>
    public void UpdateEntrop()
    {
        var sumOfWeight = SuperPosition.Sum(pattern => GetTileRule[pattern].TileWeight);
        var sumOfWeightLogWeight = SuperPosition.Sum(pattern => GetTileRule[pattern].TileWeight * Mathf.Log(GetTileRule[pattern].TileWeight));
        Entrop = Mathf.Log(sumOfWeight) - sumOfWeightLogWeight / sumOfWeight;
    }

}
