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
        public Vector2Int position;           // room (0,0) (0,1),...
        public RectInt tileSize;
        public List<int> neighbors;

        public Vector2Int GetDirectionToRoom(RoomData room)
        {
            Vector2Int dir = new Vector2Int();
            if (neighbors.Contains(room.idx))
            {
                dir = room.position - position;
            }
            else
            {
                DebugManager.Log($"!error {idx} to {room.idx} : {neighbors.Count}");
                foreach (var index in neighbors)
                {
                    DebugManager.Log($"{index}");
                }
            }
            return dir;
        }

        public TileData.TileType GetTileType(int x, int y)
        {
            if (tileSize.xMin == x || tileSize.xMax - 1 == x || tileSize.yMin == y || tileSize.yMax - 1 == y)
            {
                return TileData.TileType.Wall;
            }
            return TileData.TileType.Floor;
        }

        public RoomData(RoomData data)
        {
            idx = data.idx;
            position = data.position;
            tileSize = data.tileSize;
            neighbors = new List<int>(data.neighbors);
        }
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

    private Vector2Int _tilemapSize = new Vector2Int();
    private Vector2Int _roomMaxCount = new Vector2Int();

    const int ROOM_MIN_SIZE_X = 5;
    const int ROOM_MIN_SIZE_Y = 5;

    // empty tile count between each rooms
    const int WALL_MIN_THICKNESS = 1;

    public int floorIndex = 24;

    [SerializeField] private bool IsDebugTest = false;
    [SerializeField] private bool IsDebugClear = false;

    // Start is called before the first frame update
    public void GenerateTilemap()
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

        if (IsDebugClear)
        {
            return;
        }
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
        _tilemapSize.x = _tilemapData.width;
        _tilemapSize.y = _tilemapData.height;

        // total rooms = roomColumnIdxMax * roomRowIdxMax
        _roomMaxCount.x = _tilemapData.room_width;
        _roomMaxCount.y = _tilemapData.room_height;

        Vector2Int roomMaxSize = new Vector2Int();
        roomMaxSize.x = Mathf.FloorToInt((_tilemapSize.x - (_roomMaxCount.x - 1) * WALL_MIN_THICKNESS) / _roomMaxCount.x);
        roomMaxSize.y = Mathf.FloorToInt((_tilemapSize.y - (_roomMaxCount.y - 1) * WALL_MIN_THICKNESS) / _roomMaxCount.y);

        DebugManager.Log($"Room: {roomMaxSize.ToString()}/ Tilemap: {_tilemapSize.ToString()}");
        
        /*
         *  +y
         *  | (idx = 2n),..
         *  | (idx = n)
         *  | (idx = 0), (idx = 1),..
         *  -----------> +x
         */
        for (var positionY = 0; positionY < _roomMaxCount.y; positionY++)
        {
            for (var positionX = 0; positionX < _roomMaxCount.x; positionX++)
            {
                int roomSizeX = Random.Range(ROOM_MIN_SIZE_X, roomMaxSize.x);
                int roomSizeY = Random.Range(ROOM_MIN_SIZE_Y, roomMaxSize.y);
                
                if (!OmniEveIsNormalStageFloor(floorIndex))
                {
                    roomSizeX = roomMaxSize.x;
                    roomSizeY = roomMaxSize.y;
                }

                RoomData room = new RoomData();
                room.idx = _listRoomData.Count;
                room.position = new Vector2Int(positionX, positionY);

                Vector2Int offset = new Vector2Int();
                offset.x = Random.Range(0, roomMaxSize.x - roomSizeX);
                offset.y = Random.Range(0, roomMaxSize.y - roomSizeY);

                RectInt tileSize = new RectInt();
                tileSize.x = offset.x + (roomMaxSize.x + WALL_MIN_THICKNESS) * positionX;
                tileSize.y = offset.y + (roomMaxSize.y + WALL_MIN_THICKNESS) * positionY;
                tileSize.size = new Vector2Int(roomSizeX, roomSizeY);
                room.tileSize = tileSize;

                List<int> neighbors = new List<int>();
                    
                // left
                if (positionX > 0)
                {
                    neighbors.Add(room.idx - 1);
                }
                // top
                if (positionY < _roomMaxCount.y - 1)
                {
                    neighbors.Add(room.idx + _roomMaxCount.x);
                }
                // right
                if (positionX < _roomMaxCount.x - 1)
                {
                    neighbors.Add(room.idx + 1);
                }
                // bottom
                if (positionY > 0)
                {
                    neighbors.Add(room.idx - _roomMaxCount.x);
                }
                room.neighbors = neighbors;

                //DebugManager.Log($"room: {room.idx}({positionX}, {positionY}) = {room.idx - 1}, {room.idx + _roomMaxCount.x}, {room.idx + 1}, {room.idx - _roomMaxCount.x} : total {neighbors.Count}");
                OmniEveSetTileFloorByRoom(room);
                _listRoomData.Add(room);
            }
        }
    }
    
    protected void OmniEveSetTileFloorByRoom(RoomData room)
    {
        var tileSize = room.tileSize;
        for (var y = tileSize.yMin; y < tileSize.yMax; y++)
        {
            for (var x = tileSize.xMin; x < tileSize.xMax; x++)
            {
                TileData.TileType tileType = room.GetTileType(x, y);
                SetTile(x, y, tileType);
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

        if (TileData.TileType.Wall == tileType)
        {
            if (_dictTileData.ContainsKey(data.GetKey()))
            {
                return;
            }
        }

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
        List<RoomData> listRoomData = _listRoomData.OrderBy(a => Guid.NewGuid()).ToList().ConvertAll(x => new RoomData(x));

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
        Rect dstRect = Rect.MinMaxRect(dst.tileSize.xMin + 2, dst.tileSize.yMin + 2, dst.tileSize.xMax - 2, dst.tileSize.yMax - 2);
        Vector2Int dstPosition = new Vector2Int(Random.Range((int)dstRect.xMin, (int)dstRect.xMax), Random.Range((int)dstRect.yMin, (int)dstRect.yMax));
        Vector2Int srcPosition = new Vector2Int(Random.Range(src.tileSize.xMin +1, src.tileSize.xMax -1), Random.Range(src.tileSize.yMin +1, src.tileSize.yMax -1));
        
        // set direction
        int dirX = Math.Clamp(dstPosition.x - srcPosition.x, -1, 1);
        int dirY = Math.Clamp(dstPosition.y - srcPosition.y, -1, 1);
        
        // srcPosition to dstPosition
        Vector2Int position = srcPosition;
        while (!dstRect.Contains(position))
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    TileData.TileType tileType = TileData.TileType.Wall;
                    if (x == 0 && y == 0)
                    {
                        tileType = TileData.TileType.Floor;
                    };
                    SetTile(position.x + x, position.y + y, tileType);
                }
            }
            
            if (position.y != dstPosition.y && position.x >= (dstPosition.x - srcPosition.x) * 0.5)
            {
                position.y += dirY;
            }
            else
            {
                position.x += dirX;
            }
        }
        // DebugManager.Log($"{src.idx} - {src.position.ToString()} = ({src.GetDirectionToRoom(dst)}) to {dst.idx} - {dst.position.ToString()} = ({dst.GetDirectionToRoom(src)})");
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