using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Surroundings",menuName = "创建群落")]
public class Biochemical : ScriptableObject
{
    public Sprite[] spr;

    public TileBase tile;
    public float minHeight;
    public float minMoisture;
    public float minHeat;
    /// <summary>
    ///随机获取tile图片
    /// </summary>
    /// <returns></returns>
    public Sprite GetTleSprite()
    {
        return spr[Random.Range(0, spr.Length)];
    }

    /// <summary>
    /// 看当前生物群落是否符合环境
    /// </summary>
    /// <param name="height"></param>
    /// <param name="moisture"></param>
    /// <param name="heat"></param>
    /// <returns></returns>
    public bool MatchCondition(float height, float moisture, float heat)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
    }
}
