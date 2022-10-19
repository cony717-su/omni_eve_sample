using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using TileData = TilemapGenerator.TileData;

public class LevelManager : IManager<LevelManager>
{
    Tilemap _tilemap;
    TilemapGenerator _tilemapGenerator;
    Dictionary<int, TileData> _dictTileData;

    [SerializeField] private bool IsDebugTest = false;
    [SerializeField] private bool IsDebugClear = false;

    void Init()
    {
        _tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
        _tilemapGenerator = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapGenerator>();
    }

    public void GenerateTilemap()
    {
        if (_tilemap == null || _tilemapGenerator == null) Init();
        _dictTileData = _tilemapGenerator.GenerateTilemap(IsDebugTest, IsDebugClear);

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

[CustomEditor(typeof(LevelManager))]
public class TilemapGeneratorEditor : Editor
{
    private LevelManager levelManager;

    private void OnEnable()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Tilemap"))
        {
            // generate
            levelManager.GenerateTilemap();
        }
    }
}
