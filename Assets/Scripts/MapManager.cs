//----------------------------------------------
// File: LoadMap.cs
// Copyright © 2018 InsertCoin (www.insertcoin.info)
// Author: Omer Akyol
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.IO;
using Astar;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour 
{
    #region EditorFields

    public float VisualizeSpeed = 0.1f;
    public Transform MapTrans;
    public GameObject AgentPrefab;
    public GameObject ObstacklePrefab;
    public GameObject PathPrefab;
    public GameObject GoalPrefab;
    public GameObject TilePrefab;

    #endregion

    #region Fields

    public const int MapSize = 100;
    private int[,] Map = new int[MapSize, MapSize];
    private Tile mStartTileObj;
    private Tile mEndTileObj;
    private Camera mMainCam;
    private bool mMouse1;
    private bool mMouse2;
    private bool mObstackleBrush;
    private bool mStartBrush;
    private bool mEndBrush;

    #endregion

    #region UnityMethods

    void Awake()
    {
        mMainCam = Camera.main;
    }

    void Start () 
    {
        GenerateMap();
    }

    void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetSimulation();
        }

        mMouse1 = Input.GetMouseButton(0);
        mMouse2 = Input.GetMouseButton(1);

        if (mMouse1 || mMouse2)
        {
            PaintUpdate();
        }
    }

    #endregion

    #region Public

    public bool StartSimulation(out string errorMessage)
    {
        bool result  = false;
        errorMessage = "Start and End Tiles\nare not defined!";

        if(mStartTileObj != null && mEndTileObj != null)
        {
            PathFinder pf = new PathFinder();
            var path = pf.Astar(Map, mStartTileObj.Pos, mEndTileObj.Pos);

            // Instant visualization
            // VisualizePath(path);

            if (path != null)
            {
                // Incremantal visualization
                StartCoroutine(VisualizePathRoutine(path));
                errorMessage = "Path found :)";
                result = true;
            }
            else
            {
                errorMessage = "Can't find path!";
                result = false;
            }
        }

        return result;
    }

    public void ResetSimulation()
    {
        SceneManager.LoadScene(0);
    }

    public void SetBrush(bool obstackle, bool start, bool end)
    {
        mObstackleBrush = obstackle;
        mStartBrush     = start;
        mEndBrush       = end;
    }

    #endregion

    #region Private

    void PaintUpdate()
    {
        var ray = mMainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.tag == "Tile")
            {
                var tile = hitInfo.collider.gameObject.GetComponent<Tile>();

                if (mMouse1) // Paint
                {
                    if (tile.TType == TileType.Empty)
                    {
                        if (mObstackleBrush)
                        {
                            Map[tile.Pos.y, tile.Pos.x] = (int)TileType.Obstackle;
                            GameObject.Instantiate(ObstacklePrefab, MapTrans).GetComponent<Tile>().SetPos(tile.Pos);
                        }
                        else if (mStartBrush)
                        {
                            if(mStartTileObj != null)
                            {
                                Map[mStartTileObj.Pos.y, mStartTileObj.Pos.x] = (int)TileType.Empty;
                                GameObject.Destroy(mStartTileObj.gameObject);
                            }

                            mStartTileObj = GameObject.Instantiate(AgentPrefab, MapTrans).GetComponent<Tile>();
                            mStartTileObj.SetPos(tile.Pos);
                        }
                        else if (mEndBrush)
                        {
                            if(mEndTileObj != null)
                            {
                                Map[mEndTileObj.Pos.y, mEndTileObj.Pos.x] = (int)TileType.Empty;
                                GameObject.Destroy(mEndTileObj.gameObject);
                            }

                            Map[tile.Pos.y, tile.Pos.x] = (int)TileType.Goal;
                            mEndTileObj = GameObject.Instantiate(GoalPrefab, MapTrans).GetComponent<Tile>();
                            mEndTileObj.SetPos(tile.Pos);
                        }
                    }
                }
                else if (mMouse2) // Delete
                {
                    if (tile.TType != TileType.Empty)
                    {
                        if (tile.TType == TileType.Agent)
                            mStartTileObj = null;
                        else if (tile.TType == TileType.Goal)
                            mEndTileObj = null;

                        Map[tile.Pos.y, tile.Pos.x] = (int)TileType.Empty;
                        GameObject.Destroy(tile.gameObject);
                    }
                }

            }
        }
    }

    void VisualizePath(List<Node> path)
    {
        foreach(var node in path)
        {
            GameObject.Instantiate(PathPrefab, MapTrans).transform.position = new Vector3(node.Position.x, +0.25f, -node.Position.y);
        }
    }

    IEnumerator VisualizePathRoutine(List<Node> path)
    {
        foreach (var node in path)
        {
            GameObject.Instantiate(PathPrefab, MapTrans).transform.position = new Vector3(node.Position.x, +0.25f, -node.Position.y);
            yield return new WaitForSeconds(VisualizeSpeed);
        }
    }

    void GenerateMap()
    {
        GenerateEmptyMap();
    }

    void GenerateEmptyMap()
    {
        for (int y = 0; y < MapSize; y++)
        {
            for (int x = 0; x < MapSize; x++)
            {
                Map[y, x] = (int)TileType.Empty;
                GameObject.Instantiate(TilePrefab, MapTrans).GetComponent<Tile>().SetPos(new Vector2Int(x, y));
            }
        }

        Map[0, 0] = (int)TileType.Agent;
        mStartTileObj = GameObject.Instantiate(AgentPrefab, MapTrans).GetComponent<Tile>();
        mStartTileObj.SetPos(new Vector2Int(0, 0));

        Map[MapSize - 2, MapSize - 2] = (int)TileType.Goal;
        mEndTileObj = GameObject.Instantiate(GoalPrefab, MapTrans).GetComponent<Tile>();
        mEndTileObj.SetPos(new Vector2Int(MapSize - 2, MapSize - 2));
    }

    #endregion
}
