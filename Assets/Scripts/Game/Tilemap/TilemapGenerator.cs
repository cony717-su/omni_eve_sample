using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

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

    public RuleTile tileBase;
    public int floorIndex = 24;
    
    
    // Start is called before the first frame update
    public void GenerateTilemap(int floorIndex = 1)
    {
        _tilemapData = StaticManager.Instance.Get<StaticOmniEveFloor>(floorIndex);
        Debug.Log($"Load MapData: {_tilemapData.width}, {_tilemapData.height}");

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
                if (col > 1)
                {
                    neighbors.Add(room.idx - 1);
                }

                // right
                if (col < roomColumnIdxMax)
                {
                    neighbors.Add(room.idx + 1);
                }

                // top
                if (row > 1)
                {
                    neighbors.Add(room.idx - roomColumnIdxMax);
                }

                // bottom
                if (row < roomRowIdxMax)
                {
                    neighbors.Add(room.idx + roomColumnIdxMax);
                }

                room.neighbors = neighbors;

                OmniEveSetTileFloorByRoom(room);
                _listRoomData.Add(room);
            }
        }
    }
    
    protected void OmniEveSetTileFloorByRoom(RoomData room)
    {
        DebugManager.Log($"room {room.idx} ({room.left}, {room.right}, {room.top}, {room.bottom})");

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
        int startIndex = Random.Range(0, _listRoomData.Count);
        List<RoomData> connectedRoom;


        /*List<int> unconnectedRoomIdxList = new List<int>();
        List<int> connectedRoomIdxList = new List<int>();

        foreach (var room in _listRoomData)
        {
            unconnectedRoomIdxList.Add(room.idx);
        }

        int unconnectedCount = unconnectedRoomIdxList.Count - 1;
        connectedRoomIdxList.Add(unconnectedRoomIdxList[unconnectedCount]);

        unconnectedRoomIdxList.RemoveAt(unconnectedCount);
        unconnectedCount = unconnectedRoomIdxList.Count - 1;

        while (unconnectedCount > 0)
        {
            int srcIdx = connectedRoomIdxList[Random.Range(0, connectedRoomIdxList.Count)];
            RoomData connectedRoom = OmniEveGetRoom(srcIdx);

            int randomIdx = Random.Range(0, connectedRoom.neighbors.Count);
            int dstIdx = connectedRoom.neighbors[Random.Range(0, connectedRoom.neighbors.Count)];

            int roomIdx = connectedRoomIdxList.Find(x => x == dstIdx);
            if (roomIdx <= 0)
            {
                GeneratePassageByRoomIdx(srcIdx, dstIdx);

                unconnectedRoomIdxList.Remove(dstIdx);
                connectedRoomIdxList.Add(dstIdx);

                unconnectedCount = unconnectedRoomIdxList.Count - 1;
            }
        }*/
    }

    protected void OmniEveConnectRoom(RoomData src, RoomData dst)
    {
        // todo
        DebugManager.Log($"{src.idx} to {dst.idx}");
        int srcX = Random.Range(src.left + 1, src.right - 1); 
        int srcY = src.bottom;
        int dstX = Random.Range(dst.left + 1, dst.right - 1); 
        int dstY = dst.top;
        int centerY = Random.Range(srcY + 1, dstY - 1);

        Vector2Int srcPosition = new Vector2Int(Random.Range(src.left +1, src.right -1), Random.Range(src.top +1, src.bottom -1));
        Vector2Int dscPosition = new Vector2Int(Random.Range(dst.left +1, dst.right -1), Random.Range(dst.top +1, dst.bottom -1));
        Vector2Int center = srcPosition - dscPosition;
        center.x /= 2;
        center.y /= 2;

        int dirX = center.x > 0 ? 1 : -1; 
        for (int i = 0; i < Mathf.Abs(center.x) ; i++)
        {
            int posX = srcPosition.x + i * dirX;
            int posY = srcPosition.y;
            SetTile(posX, posY, TileData.TileType.Floor);
        }
        
        int dirY = center.y > 0 ? 1 : -1;
        for (int i = 0; i < Mathf.Abs(center.y) ; i++)
        {
            
        }
    }

    protected void GeneratePassageByRoomIdx(int srcIdx, int dstIdx)
    {
        RoomData src = OmniEveGetRoom(srcIdx);
        RoomData dst = OmniEveGetRoom(dstIdx);

        OmniEveConnectRoom(src, dst);
        return;
        
        if (src.row == dst.row)
        {
            // left to right
            OmniEveConnectRoomLeftToRight(src, dst);
        }
        else
        {
            // top to bottom
            OmniEveConnectRoomTopToBottom(src, dst);
        }
    }

    protected void OmniEveConnectRoomTopToBottom(RoomData src, RoomData dst)
    {
        int srcX = Random.Range(src.left + 1, src.right - 1); 
        int srcY = src.bottom;
        int dstX = Random.Range(dst.left + 1, dst.right - 1); 
        int dstY = dst.top;
        int centerY = Random.Range(srcY + 1, dstY - 1);

        // (+ 1) (-1) �� Wall Ÿ���� �����ϱ� ���ؼ���
        for (var y = srcY; y <= centerY + 1; y++)
        {
            if (y <= centerY)
            {
                SetTile(y, srcX, TileData.TileType.Passage);
            }

            // top side of passage line
            TileData data;
            if (GetTileByRowColumn(y, srcX - 1, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(y, srcX - 1, TileData.TileType.Wall);
                }
            }

            // bottom side of passage line
            if (GetTileByRowColumn(y, srcX + 1, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(y, srcX + 1, TileData.TileType.Wall);
                }
            }
        }

        for (var y = centerY - 1; y <= dstY; y++)
        {
            if (y >= centerY)
            {
                SetTile(y, dstX, TileData.TileType.Passage);
            }

            TileData data;
            if (GetTileByRowColumn(y, dstX - 1, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(y, dstX - 1, TileData.TileType.Wall);
                }
            }

            // bottom side of passage line
            if (GetTileByRowColumn(y, dstX + 1, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(y, dstX + 1, TileData.TileType.Wall);
                }
            }
        }

        int dx = 1;
        if (srcX > dstX)
        {
            dx = -1;
        }

        for (var i = 0; i < Mathf.Abs(dstX - srcX); i++)
        {
            int x = (i * dx) + srcX;
            SetTile(centerY, x, TileData.TileType.Passage);


            TileData data;
            if (GetTileByRowColumn(centerY + 1, x, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(centerY + 1, x, TileData.TileType.Wall);
                }
            }

            // bottom side of passage line
            if (GetTileByRowColumn(centerY - 1, x, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(centerY - 1, x, TileData.TileType.Wall);
                }
            }
        }
    }

    protected void OmniEveConnectRoomLeftToRight(RoomData src, RoomData dst)
    {
        int srcX = src.right;
        int srcY = Random.Range(src.top + 1, src.bottom - 1);
        int dstX = dst.left;
        int dstY = Random.Range(dst.top + 1, dst.bottom - 1);
        int centerX = Random.Range(srcX + 1, dstX - 1);

        for (var x = srcX; x <= centerX + 1; x++)
        { 
            if (x <= centerX)
            {
                SetTile(srcY, x, TileData.TileType.Passage);
            }

            // top side of passage line
            TileData data;
            if (GetTileByRowColumn(srcY - 1, x, out data))
            {
                Debug.Log($"{data.tileType}");
                if(!data.IsPassable())
                {
                    SetTile(srcY - 1, x, TileData.TileType.Wall);
                }
            }

            // bottom side of passage line
            if (GetTileByRowColumn(srcY + 1, x, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(srcY + 1, x, TileData.TileType.Wall);
                }
            }
        }

        for (var x = centerX - 1; x <= dstX; x++)
        {
            if (x >= centerX)
            {
                SetTile(dstY, x, TileData.TileType.Passage);
            }

            TileData data;
            if (GetTileByRowColumn(dstY - 1, x, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(dstY - 1, x, TileData.TileType.Wall);
                }
            }

            // bottom side of passage line
            if (GetTileByRowColumn(dstY + 1, x, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(dstY + 1, x, TileData.TileType.Wall);
                }
            }
        }

        int dy = 1;
        if (srcY > dstY)
        {
            dy = -1;
        }

        for (var i = 0; i < Mathf.Abs(dstY - srcY); i++)
        {
            int y = (i * dy) + srcY;
            SetTile(y, centerX, TileData.TileType.Passage);

            TileData data;
            if (GetTileByRowColumn(y, centerX - 1, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(y, centerX - 1, TileData.TileType.Wall);
                }
            }

            // bottom side of passage line
            if (GetTileByRowColumn(y, centerX + 1, out data))
            {
                if (!data.IsPassable())
                {
                    SetTile(y, centerX + 1, TileData.TileType.Wall);
                }
            }
        }
    }


    protected bool GetTileByRowColumn(int row, int col, out TileData data)
    {
        int key = TileData.GetKey(row, col);
        return _dictTileData.TryGetValue(key, out data);
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