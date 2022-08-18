using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapGenerator : MonoBehaviour
{
    public struct RoomData
    {
        public int idx;
        public int row;
        public int col;
        public int left, top, right, bottom;
        public List<int> neighbors;
    }

    public struct TileData
    {
        public enum TileType {
            Floor,
            Wall,
            Passage,
            Shop,
            Stair,
            Block,
        };
        public TileType tileType;
        public Vector3Int position;


        public bool IsPassable()
        {
            switch(tileType)
            {
                case TileType.Floor:
                case TileType.Passage:
                case TileType.Stair:
                case TileType.Shop:
                    return true;
                default:
                    return false;
            }
        }

        public int GetKey()
        {
            return position.x << 10 | position.y;
        }

        public static int GetKey(int row, int col)
        {
            return row << 10 | col;
        }
    }
    [SerializeField] private RuleTileBase tileFloor;
    [SerializeField] private RuleTileBase tileWall;
    [SerializeField] private RuleTileBase tileBase;
    
    private Tilemap _tilemap;
    private StaticOmniEveFloor _tilemapData;
    private List<RoomData> _listRoomData;
    Dictionary<int, TileData> _dictTileData = new Dictionary<int, TileData>();
    
    int tileRowIdxMax = 0;
    int tileColumnIdxMax = 0;
    int roomRowIdxMax = 0;
    int roomColumnIdxMax = 0;

    const int ROOM_SIZE_X_MIN = 5;
    const int ROOM_SIZE_Y_MIN = 5;
    
    // empty tile count between each rooms
    const int WALL_THICKNESS_MIN = 1;

    public int floorIndex = 24;

    [SerializeField] private bool IsDebugTest = false;

    // Start is called before the first frame update
    public void GenerateTilemap(int floorIndex = 1)
    {
        _tilemapData = StaticManager.Instance.Get<StaticOmniEveFloor>(floorIndex);

        if (IsDebugTest)
        {
            _tilemapData = new StaticOmniEveFloor();
            _tilemapData.width = 32;
            _tilemapData.height = 32;
        
            _tilemapData.room_width = 3;
            _tilemapData.room_height = 3;
            Debug.Log($"Load MapData: {_tilemapData.width}, {_tilemapData.height}");            
        }
        
        _tilemap = GetComponent<Tilemap>();
        _tilemap.ClearAllTiles();
        _dictTileData.Clear();

        GenerateRooms();
        GeneratePassages();
    }

    protected bool OmniEveIsNormalStageFloor(int floor)
    {
        // todo
        return true;
    }
    
    
    protected void GenerateRooms()
    {
        _listRoomData = new List<RoomData>();

        // tilemap size = tileColumnIdxMax * tileRowIdxMax
        tileColumnIdxMax = _tilemapData.width;
        tileRowIdxMax = _tilemapData.height;
        
        // total rooms = roomColumnIdxMax * roomRowIdxMax
        roomColumnIdxMax = _tilemapData.room_width;
        roomRowIdxMax = _tilemapData.room_height;

        int roomSizeXMax = Mathf.FloorToInt((tileColumnIdxMax - (roomColumnIdxMax - 1) * WALL_THICKNESS_MIN) / roomColumnIdxMax);
        int roomSizeYMax = Mathf.FloorToInt((tileRowIdxMax - (roomRowIdxMax - 1) * WALL_THICKNESS_MIN) / roomRowIdxMax);

        for (var row = 0; row < roomRowIdxMax; row++)
        {
            for (var col = 0; col < roomColumnIdxMax; col++)
            {
                int sizeX = Random.Range(ROOM_SIZE_X_MIN, roomSizeXMax);
                int sizeY = Random.Range(ROOM_SIZE_Y_MIN, roomSizeYMax);

                if (!OmniEveIsNormalStageFloor(floorIndex))
                {
                    sizeX = roomSizeXMax;
                    sizeY = roomSizeYMax;
                }

                int localLeft = Random.Range(0, roomSizeXMax - sizeX);
                int localTop = Random.Range(0, roomSizeYMax - sizeY);

                int left = (roomSizeXMax + WALL_THICKNESS_MIN) * col + localLeft;
                int top = (roomSizeYMax + WALL_THICKNESS_MIN) * row + localTop;

                RoomData room = new RoomData();
                room.idx = _listRoomData.Count;
                room.row = row;
                room.col = col;

                // Tile coordinates (includes wall tiles)
                room.left = left;
                room.top = top;
                room.right = left + sizeX;
                room.bottom = top + sizeY;

                List<int> neighbors = new List<int>();
                    
                // left
                if (col > 0)
                {
                    neighbors.Add(room.idx - 1);
                }

                // right
                if (col < roomColumnIdxMax - 1)
                {
                    neighbors.Add(room.idx + 1);
                }

                // top
                if (row < roomRowIdxMax - 1)
                {
                    neighbors.Add(room.idx + roomColumnIdxMax);
                }

                // bottom
                if (row > 0)
                {
                    neighbors.Add(room.idx - roomColumnIdxMax);
                }

                room.neighbors = neighbors;
                
                OmniEveSetTileFloorByRoom(room);
                _listRoomData.Add(room);
            }
        }
    }
    
    protected void OmniEveSetTileFloorByRoom(RoomData room)
    {
        for (var row = room.top; row < room.bottom; row++)
        {
            for (var col = room.left; col < room.right; col++)
            {
                TileData.TileType tileType = TileData.TileType.Floor;
                if (row == room.top || row == room.bottom -1 || col == room.left || col == room.right -1)
                {
                    tileType = TileData.TileType.Wall;
                }
                SetTile(col, row, tileType);
            }
        }
    }

    protected void SetTile(int posX, int posY, TileData.TileType tileType = TileData.TileType.Floor)
    {
        RuleTileBase ruleTile;
        switch (tileType)
        {
            case TileData.TileType.Wall:
                ruleTile = tileWall;
                break;
            case TileData.TileType.Passage:
                ruleTile = tileBase;
                break;
            default:
                ruleTile = tileFloor;
                break;
        }
        
        TileData data = new TileData();
        data.position = new Vector3Int(posX, posY, 0);
        data.tileType = tileType;

        _dictTileData.TryAdd(data.GetKey(), data);
        _tilemap.SetTile(data.position, ruleTile);
    }

    protected RoomData OmniEveGetRoom(int roomIdx)
    {
        return _listRoomData.Find(x => x.idx == roomIdx);
    }

    protected void GeneratePassages()
    {
        // pool
        List<RoomData> listRoomData = new List<RoomData>(_listRoomData.OrderBy(a => Guid.NewGuid()).ToList());

        // get items
        List<int> listRoomIndex = new List<int>();
        for (int i = 0; i < listRoomData.Count; i++)
        {
            listRoomIndex.Add(i);
        }

        Dictionary<int, RoomData> dictNode = new Dictionary<int, RoomData>();
        while (listRoomIndex.Count > 0)
        {
            int elementIndex = listRoomIndex.First();
            RoomData room = listRoomData[elementIndex];

            bool isNewNode = false;
            List<int> listNeighbor = new List<int>(room.neighbors.OrderBy(a => Guid.NewGuid()).ToList());
            foreach (var nodeIndex in listNeighbor)
            {
                RoomData root;
                if (dictNode.TryGetValue(nodeIndex, out root) || dictNode.Count == 0)
                {
                    // already exist
                    room.neighbors.Remove(nodeIndex);

                    if (!dictNode.ContainsKey(room.idx))
                    {
                        root.neighbors?.Add(room.idx);
                        listRoomIndex.RemoveAt(0);
                        
                        RoomData node = new RoomData();
                        node.idx = room.idx;
                        node.neighbors = new List<int>();
                        dictNode.Add(node.idx, node);

                        isNewNode = true;
                    }
                    break;
                }
            }

            if (!isNewNode)
            {
                // move to next a room
                listRoomIndex.RemoveAt(0);
                listRoomIndex.Add(elementIndex);
            }
        }

        foreach (var room in dictNode.Values)
        {
            foreach (var dstIdx in room.neighbors)
            {
                RoomData src = OmniEveGetRoom(room.idx);
                RoomData dst = OmniEveGetRoom(dstIdx);
                OmniEveConnectRoom(src, dst);
            }
        }
    }

    protected void OmniEveConnectRoom(RoomData src, RoomData dst)
    {
        Vector2Int srcPosition = new Vector2Int(Random.Range(src.left +1, src.right -1), Random.Range(src.top +1, src.bottom -1));
        Vector2Int dstPosition = new Vector2Int(Random.Range(dst.left +1, dst.right -1), Random.Range(dst.top +1, dst.bottom -1));
        
        Vector2 centerf = dstPosition - srcPosition;
        centerf.x /= 2;
        centerf.y /= 2;

        Vector2Int center = new Vector2Int((int)(centerf.x), (int)(centerf.y));
        int dirX = centerf.x > 0 ? 1 : -1;
        for (int i = 0; i < Mathf.Abs(center.x) ; i++)
        {
            SetTile(srcPosition.x + i * dirX, srcPosition.y, TileData.TileType.Floor);
            SetTile(srcPosition.x + center.x + i * dirX, dstPosition.y, TileData.TileType.Floor);
        }

        int dirY = centerf.y > 0 ? 1 : -1;
        for (int i = 0; i <= Mathf.Abs(center.y) ; i++)
        {
            SetTile(srcPosition.x + center.x, srcPosition.y + i * dirY, TileData.TileType.Floor);
            SetTile(srcPosition.x + center.x, srcPosition.y + center.y + i * dirY, TileData.TileType.Floor);
        }
        
        //DebugManager.Log($"{src.idx} ({srcPosition.x}, {srcPosition.y}) to {dst.idx} ({dstPosition.x}, {dstPosition.y}) = {center.x}, {center.y}/ {centerf.x}, {centerf.y}");
    }
}

[CustomEditor(typeof(TilemapGenerator))]
public class TilemapGeneratorEditor : Editor
{
    private TilemapGenerator generator;

    private void OnEnable()
    {
        generator = FindObjectOfType<TilemapGenerator>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Tilemap"))
        {
            // generate
            generator.GenerateTilemap();
        }
    }
}