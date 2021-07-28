using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Surroundings",menuName = "����Ⱥ��")]
public class Biochemical : ScriptableObject
{
    public Sprite[] tiles;
    public float minHeight;
    public float minMoisture;
    public float minHeat;
    /// <summary>
    ///�����ȡtileͼƬ
    /// </summary>
    /// <returns></returns>
    public Sprite GetTleSprite()
    {
        return tiles[Random.Range(0, tiles.Length)];
    }

    /// <summary>
    /// ����ǰ����Ⱥ���Ƿ���ϻ���
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
