//----------------------------------------------
// File: Tile.cs
// Copyright © 2018 InsertCoin (www.insertcoin.info)
// Author: Omer Akyol
//----------------------------------------------

using UnityEngine;
using System.Collections;

public enum TileType
{
    Empty = 0, 
    Obstackle,
    Goal,
    Agent
}

public class Tile : MonoBehaviour 
{
    #region EditorFields

    public TileType TType;
    public Vector2Int Pos;

    #endregion

    public void SetPos(Vector2Int p)
    {
        Pos = p;
        float yShift = TType == TileType.Empty ? -0.25f : 0.5f;
        transform.localPosition = new Vector3(Pos.x, yShift, -Pos.y);
    }
}
