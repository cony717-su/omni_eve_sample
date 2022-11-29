using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = TilemapGenerator.TileData;

public class LevelManager : IManager<LevelManager>
{
    Tilemap _tilemap;
    TilemapGenerator _tilemapGenerator;
    Dictionary<int, TileData> _dictTileData;

    void Init()
    {
        _tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
        _tilemapGenerator = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapGenerator>();
    }

    public void GenerateTilemap(bool isDebugTest, bool isDebugClear)
    {
        if (_tilemap == null || _tilemapGenerator == null) Init();
        _dictTileData = _tilemapGenerator.GenerateTilemap(isDebugTest, isDebugClear);

        TileData stairTile = OmniEveGetRandomTile(true);
        _tilemapGenerator.OmniEveSetStairs(stairTile);
    }

    public void OmniEveSetTilemapTilePosition(TileData tilePos)
    {
        Vector3Int shift = new Vector3Int(4, 10, 0) - tilePos.position;
        _tilemap.transform.position += shift;
    }

    public TileData OmniEveGetTileDataByRowColumn(int row, int col)
    {
        return _tilemapGenerator.OmniEveGetTileDatabyRowColumn(row, col);
    }

    public TileData OmniEveGetRandomTile(bool isStairs)
    {
        return _tilemapGenerator.OmniEveGetRandomTile(isStairs);
    }

    public bool OmniEveIsRangeInDistance(Vector3Int monsterTilePos, Vector3Int characterTilePos, int range)
    {
        return true;
    }
}

