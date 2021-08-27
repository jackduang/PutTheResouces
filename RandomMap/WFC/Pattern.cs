using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Linq;
using Sirenix;
using Sirenix.OdinInspector;

//先观察熵的最小值，再对其进行坍缩

namespace MyWfcTest
{
    public class ModelRulesInfor
    {
        public float TileWeight = 0;
        public Dictionary<int, HashSet<TileBase>> Dic_DirTiles;
        public ModelRulesInfor()
        {
            Dic_DirTiles = new Dictionary<int, HashSet<TileBase>>();
            for (int i = 0; i < 6; i++)
            {
                Dic_DirTiles.Add(i, new HashSet<TileBase>());
            }
        }
    }

    [System.Serializable]
    public class NullInfor
    {
        public bool OpenNullGird;
        public TileBase NullBase;
        public float NullWeight = 0.1f;
    }
    public class Pattern : SerializedMonoBehaviour
    {

        // Start is called before the first frame update
        public BoundsInt bounds;

        public Tilemap[] tilemap;

        public NullInfor[] nullInfor;

        public Dictionary<TileBase, float> TileWeight = new Dictionary<TileBase, float>();

        [HideInInspector] public Dictionary<TileBase, ModelRulesInfor> TileInfor = new Dictionary<TileBase, ModelRulesInfor>();
        // int AllTilesCount = 0;
        // public HashSet<TileBase> AllTiles = new HashSet<TileBase>();

        public Tile TestTile;
        private void Awake()
        {
            for (int i = 0; i < nullInfor.Length; i++)
            {
                if (nullInfor[i].NullBase == null)
                {
                    nullInfor[i].NullBase = Resources.Load<TileBase>("Null");
                }
            }

            // OpenNullGird = new bool[bounds.size.z];
        }
        void Start()
        {
            InitModeRules();

        }

        private void Update()
        {
            if (Input.anyKeyDown)
                Main.Instance.GeneratorWFCMap(TileInfor, bounds, tilemap);
        }

        //初始化瓷砖邻居规则信息 
        void InitModeRules()
        {
            foreach (var item in bounds.allPositionsWithin)
            {
                int layer = item.z;
                Vector3Int pos = tilemap[layer].WorldToCell(item);
                ModelRulesInfor modelRules;
                if (!tilemap[layer].HasTile(pos))
                {
                    if (nullInfor[layer].OpenNullGird)
                    {
                        AddNullTile(layer);
                        GetRoundTile(nullInfor[layer].NullBase, pos);
                    }
                    continue;
                }
                Vector3 CellPos = tilemap[layer].CellToWorld(pos);

                TileBase tile = tilemap[layer].GetTile(pos);

                if (!TileInfor.ContainsKey(tile))
                {
                    modelRules = new ModelRulesInfor() { TileWeight = 0 };
                    TileInfor.Add(tile, modelRules);
                }
                modelRules = TileInfor[tile];
                //自定义权重 
                if (TileWeight.ContainsKey(tile))
                {
                    modelRules.TileWeight += TileWeight[tile];
                }
                else
                    modelRules.TileWeight += 1f;
                GetRoundTile(tile, item);
            }


        }

        public void AddNullTile(int layer)
        {

            if (!TileInfor.ContainsKey(nullInfor[layer].NullBase))//添加空的格子
            {
                ModelRulesInfor modelRules = new ModelRulesInfor() { TileWeight = 0 };
                TileInfor.Add(nullInfor[layer].NullBase, modelRules);
            }
            else
            {
                TileInfor[nullInfor[layer].NullBase].TileWeight += nullInfor[layer].NullWeight;
            }
        }

        void GetRoundTile(TileBase ForTile, Vector3Int pos)
        {
            Vector3Int[] RoundPos = K_utility.GetSixRoundPos(pos);
            if (TestTile == (Tile)ForTile)
            {
                Debug.Log("sss");
            }
            for (int i = 0; i < RoundPos.Length; i++)
            {
                Vector3Int curPos = new Vector3Int(RoundPos[i].x, RoundPos[i].y, 0);
                if (!bounds.Contains(RoundPos[i]))
                {
                    continue;
                }
                int layer = RoundPos[i].z;


                HashSet<TileBase> List_dirTiles = TileInfor[ForTile].Dic_DirTiles[i];

                if (!tilemap[layer].HasTile(curPos))
                {
                    //添加空的格子
                    if (!nullInfor[layer].OpenNullGird)
                    {
                        continue;
                    }
                    AddNullTile(layer);
                    List_dirTiles.Add(nullInfor[layer].NullBase);
                    continue;
                }
                TileBase tile = tilemap[layer].GetTile(curPos);
                if (!TileInfor.ContainsKey(tile))
                    TileInfor.Add(tile, new ModelRulesInfor());
                List_dirTiles.Add(tile);
            }



        }
        // Update is called once per frame



        private void OnDrawGizmos()
        {
            // bounds.position = transform.position;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }


    }
}