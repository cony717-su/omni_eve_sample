using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor;
using Direction = CharacterManager.Player.Direction;

public class GameManager : IManager<GameManager>
{
    public string serverName;
    public string nickName;

    private Transform _tilemapTransform;
    private TilemapGenerator _tilemapGenerator;
    private CharacterManager _characterManager;
    private LevelManager _levelManager;

    [SerializeField] private bool IsDebugTest = false;
    [SerializeField] private bool IsDebugClear = false;

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
        _levelManager = gameObject.GetComponent<LevelManager>();
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
        if (Input.anyKeyDown)
        {
            OnOmniEvePressButton();
        }
        
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

    public void OmniEveGenerateFloor()
    {
        _levelManager.GenerateTilemap(IsDebugTest, IsDebugClear);
        OmniEveSetCharacterTileRandomPosition();
    }

    private void OmniEveSetCharacterTile(Vector3Int position)
    {
        getTilemapTransform();
        _tilemapTransform.position = new Vector3Int(4, 10, 0) - position;
    }

    private void OmniEveMoveCharacter(Vector3Int curPosition, Direction direction)
    {
        Vector3Int shift;
        switch (direction)
        {
            case Direction.Up:
                shift = new Vector3Int(0, 1, 0);
                break;
            case Direction.Down:
                shift = new Vector3Int(0, -1, 0);
                break;
            case Direction.Left:
                shift = new Vector3Int(-1, 0, 0);
                break;
            case Direction.Right:
                shift = new Vector3Int(1, 0, 0);
                break;
            default:
                return;
        }

        Vector3Int nextPosition = curPosition + shift;
        if (_characterManager.OmniEveIsMovable(nextPosition, _tilemapGenerator))
        {
            _tilemapTransform.position += new Vector3(0, -1, 0);
            _characterManager.OmniEveSetCharacterTilePosition(nextPosition);
            OmniEveSetCharacterTile(nextPosition);
        }
    }

    private void OnOmniEvePressButton()
    {
        Vector3Int curPosition = _characterManager.OmniEveGetCharacterTilePosition();
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OmniEveMoveCharacter(curPosition, Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OmniEveMoveCharacter(curPosition, Direction.Down);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OmniEveMoveCharacter(curPosition, Direction.Left);
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OmniEveMoveCharacter(curPosition, Direction.Right);
        }
    }

    protected void OmniEveSetCharacterTileRandomPosition()
    {
        getTilemapGenerator();

        TilemapGenerator.TileData tilePos = _tilemapGenerator.OmniEveGetRandomTile(false);
        _characterManager.OmniEveSetCharacterTilePosition(tilePos.position);
        OmniEveSetCharacterTile(tilePos.position);
    }
}

[CustomEditor(typeof(GameManager))]
public class TilemapGeneratorEditor : Editor
{
    private GameManager gameManager;

    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Tilemap"))
        {
            // generate
            gameManager.OmniEveGenerateFloor();
        }
    }
}