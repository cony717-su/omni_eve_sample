using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : IManager<GameManager>
{
    public string serverName;
    public string nickName;

    private Transform _tilemapTransform;
    public TilemapGenerator _tilemapGenerator;
    private CharacterManager _characterManager;
    
    void Start()
    {
        InitScene();
        DebugManager.Log();
    }
    void Init()
    {
        StaticManager.Instance.Init();
        DebugManager.Log();
        _characterManager = gameObject.GetComponent<CharacterManager>();
    }
    
    void InitScene()
    {
        string sceneName = GameSceneManager.Instance.ActiveSceneName;
        if (GameObject.Find(sceneName))
            return;

        Type sceneType = Type.GetType(sceneName);
        GameObject gameObject = new GameObject(sceneName);
        gameObject.AddComponent(sceneType);
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        GameManager.Instance.Init();
    }

    private void Update()
    {
        //if (getTilemapTransform() && getTilemapGenerator())
        //{
        //    if (!_tilemapGenerator.isSpawned)
        //    {
        //        TilemapGenerator.TileData spawnTile = _tilemapGenerator.OmniEveGetRandomTile(false);
        //        _tilemapGenerator.OmniEveSetTilemapTilePosition(spawnTile);
        //        _characterManager.OmniEveSetCharacterTilePosition(spawnTile.position);
        //        _tilemapGenerator.isSpawned = true;
        //    }
        //    OnOmniEvePressButton();
        //}
    }

    private bool getTilemapTransform()
    {
        _tilemapTransform = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Transform>();
        if (_tilemapTransform == null)
        {
            return false;
        }
        return true;
    }

    private bool getTilemapGenerator()
    {
        _tilemapGenerator = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapGenerator>();
        if (_tilemapGenerator == null)
        {
            return false;
        }
        return true;
    }

    private void OnOmniEvePressButton()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3Int curPosition = _characterManager.player.tilePosition;
            Vector3Int nextPosition = curPosition + new Vector3Int(0, 1, 0);
            if (_characterManager.OmniEveIsMovable(nextPosition, _tilemapGenerator))
            {
                _tilemapTransform.position += new Vector3(0, -1, 0);
                _characterManager.OmniEveSetCharacterTilePosition(nextPosition);
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3Int curPosition = _characterManager.player.tilePosition;
            Vector3Int nextPosition = curPosition + new Vector3Int(0, -1, 0);
            if (_characterManager.OmniEveIsMovable(nextPosition, _tilemapGenerator))
            {
                _tilemapTransform.position += new Vector3(0, 1, 0);
                _characterManager.OmniEveSetCharacterTilePosition(nextPosition);
            }
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3Int curPosition = _characterManager.player.tilePosition;
            Vector3Int nextPosition = curPosition + new Vector3Int(-1, 0, 0);
            if (_characterManager.OmniEveIsMovable(nextPosition, _tilemapGenerator))
            {
                _tilemapTransform.position += new Vector3(1, 0, 0);
                _characterManager.OmniEveSetCharacterTilePosition(nextPosition);
            }
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3Int curPosition = _characterManager.player.tilePosition;
            Vector3Int nextPosition = curPosition + new Vector3Int(1, 0, 0);
            if (_characterManager.OmniEveIsMovable(nextPosition, _tilemapGenerator))
            {
                _tilemapTransform.position += new Vector3(-1, 0, 0);
                _characterManager.OmniEveSetCharacterTilePosition(nextPosition);
            }
        }
    }

    protected void OmniEveSetCharacterTileRandomPosition()
    {
        TilemapGenerator.TileData tilePos = _tilemapGenerator.OmniEveGetRandomTile(false);
        _characterManager.OmniEveSetCharacterTilePosition(tilePos.position);
    }
}