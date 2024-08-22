using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct SpawnPosition
{
    public Vector3 PlayerSpawnPos;
    public Vector3 EggSpawnPos;
    public Vector3 ExitPos;
}

public class BSPProceduralGeneration
{
    private DungeonMapSettingsSO _settings;
    private Transform _parentTransform;
    private Tilemap _groundTilemap;
    private Tilemap _roundTreeTilemap;
    private Tilemap _roundGroundTilemap;
    private Tilemap _teleportTilemap;

    private BSPNode _rootNode;
    private List<RectInt> _rooms;
    private List<LinkTeleport> _linkTeleportDict;
    private SpawnPosition _spawnPosition;

    private Vector2Int _mapSize;
    private int _margin = 2;
    private int _roomCount;

    public BSPProceduralGeneration(DungeonMapSettingsSO settings, Transform parentTransform, Vector2Int mapSize)
    {
        _settings = settings;
        _mapSize = mapSize;
        _parentTransform = parentTransform;
        CreateTilemap();
    }
    public SpawnPosition GetSpawnPosition()
    {
        return _spawnPosition;
    }
    public List<RectInt> GetRooms()
    {
        return _rooms;
    }
    private void CreateTilemap()
    {
        // TODO 씬 내에 미리 만들어 둘까 고민중
        GameObject tilemapGameObject;

        // 방 타일맵
        tilemapGameObject = new GameObject("GroundTilemap");
        tilemapGameObject.transform.SetParent(_parentTransform, false);
        var tilemapComponent = tilemapGameObject.AddComponent<Tilemap>();
        var renderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = -2;
        var navMeshPlus = tilemapGameObject.AddComponent<NavMeshModifier>();
        navMeshPlus.overrideArea = true;
        navMeshPlus.area = 0;
        _groundTilemap = tilemapComponent;

        // 방 주변 나무 타일맵
        tilemapGameObject = new GameObject("RoundTreeTilemap");
        tilemapGameObject.transform.SetParent(_parentTransform, false);
        var tilemapComponent2 = tilemapGameObject.AddComponent<Tilemap>();
        tilemapGameObject.AddComponent<TilemapRenderer>();
        _roundTreeTilemap = tilemapComponent2;

        // 방 주변 땅 타일맵
        tilemapGameObject = new GameObject("RoundGroundTilemap");
        tilemapGameObject.transform.SetParent(_parentTransform, false);
        var tilemapComponent3 = tilemapGameObject.AddComponent<Tilemap>();
        var renderer2 = tilemapGameObject.AddComponent<TilemapRenderer>();
        var collider = tilemapGameObject.AddComponent<TilemapCollider2D>();
        var rigid = tilemapGameObject.AddComponent<Rigidbody2D>();
        tilemapGameObject.AddComponent<CompositeCollider2D>();
        rigid.bodyType = RigidbodyType2D.Static;
        collider.usedByComposite = true;
        renderer2.sortingOrder = -2;
        var navMeshPlus2 = tilemapGameObject.AddComponent<NavMeshModifier>();
        navMeshPlus2.overrideArea = true;
        navMeshPlus2.area = 1;
        _roundGroundTilemap = tilemapComponent3;

        // 텔레포트 타일맵 
        tilemapGameObject = new GameObject("TeleportTilemap");
        tilemapGameObject.transform.SetParent(_parentTransform, false);
        var tilemapComponent4 = tilemapGameObject.AddComponent<Tilemap>();
        var tilemapRenderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
        _teleportTilemap = tilemapComponent4;
    }

    public void Generate()
    {
        PerlinNoiseMap pnm = new PerlinNoiseMap(_settings.PerlinNoiseMapSettings, _parentTransform);
        GenerateDungeon();

        foreach (RectInt room in _rooms)
        {
            pnm.GenerateMap(room);
        }
        AddPaddingWithTrees();
        AddTeleport();
        List<RectInt> spawnRoom = RandomSpawnRoom.GetRandomValues(_rooms, 3);
        _spawnPosition = new SpawnPosition
        {
            PlayerSpawnPos = new Vector3((int)spawnRoom[0].center.x, (int)spawnRoom[0].center.y),
            EggSpawnPos = new Vector3((int)spawnRoom[1].center.x, (int)spawnRoom[1].center.y),
            ExitPos = new Vector3((int)spawnRoom[2].center.x, (int)spawnRoom[2].center.y),
        };
    }

    private void AddTeleport()
    {
        _linkTeleportDict.ToList().ForEach(t =>
        {
            Vector3Int leftPos = t.LeftTeleportLocation;
            Vector3Int rightPos = t.RightTeleportLocation;

            // Create a tile and set it to the tilemap
            Tile leftTile = ScriptableObject.CreateInstance<Tile>();
            leftTile.gameObject = _settings.TeleportPrefab;
            _teleportTilemap.SetTile(leftPos, leftTile);

            Tile rightTile = ScriptableObject.CreateInstance<Tile>();
            rightTile.gameObject = _settings.TeleportPrefab;
            _teleportTilemap.SetTile(rightPos, rightTile);

            // Access the game object created and modify its components

            GameObject leftGameObject = _teleportTilemap.GetInstantiatedObject(leftPos);
            GameObject RightGameObject = _teleportTilemap.GetInstantiatedObject(rightPos);

            ModifyTileObject(leftGameObject, RightGameObject);
        });
    }

    private void ModifyTileObject(GameObject leftGameObject, GameObject RightGameObject)
    {
        if (leftGameObject != null && RightGameObject != null)
        {
            var left = leftGameObject.GetComponent<Teleport>();
            var right = RightGameObject.GetComponent<Teleport>();
            if (left != null && right != null)
            {
                left.Init(right);
                right.Init(left);
            }
        }
    }


    private void AddPaddingWithTrees()
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

        foreach (var room in _rooms)
        {
            for (int x = room.x; x < room.xMax; x++)
            {
                for (int y = room.y; y < room.yMax; y++)
                {
                    roomPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int x = -_settings.Padding; x < _mapSize.x + _settings.Padding; x++)
        {
            for (int y = -_settings.Padding; y < _mapSize.y + _settings.Padding; y++)
            {
                if (x >= 0 && x < _mapSize.x && y >= 0 && y < _mapSize.y)
                {
                    if (!roomPositions.Contains(new Vector2Int(x, y)))
                    {
                        PlaceTree(x, y);
                    }
                }
                else
                {
                    PlaceTree(x, y);
                }
            }
        }
    }

    private void PlaceTree(int x, int y)
    {
        Vector3Int position = new Vector3Int(x, y, 0);
        _roundGroundTilemap.SetTile(position, _settings.RoundGroundTile);

        Tile treeTile = ScriptableObject.CreateInstance<Tile>();
        treeTile.gameObject = _settings.TreePrefab;
        _roundTreeTilemap.SetTile(position, treeTile);
    }

    private void GenerateDungeon()
    {
        while (_roomCount <= 2)
        {
            _linkTeleportDict = new List<LinkTeleport>();
            _rooms = new List<RectInt>();
            _roomCount = 0;
            _rootNode = new BSPNode(new RectInt(0, 0, _mapSize.x, _mapSize.y));
            Split(_rootNode);

            CreateRooms(_rootNode);
        }
        ConnectRooms(_rootNode);
    }

    private void Split(BSPNode node)
    {
        if (node.Room.width <= _settings.MinRoomSize * 2 + _margin && node.Room.height <= _settings.MinRoomSize * 2 + _margin) return;

        bool splitHorizontally = DetermineSplitOrientation(node);

        int max = (splitHorizontally ? node.Room.height : node.Room.width) - _settings.MinRoomSize - _margin;
        if (max <= _settings.MinRoomSize + _margin) return;

        int split = Random.Range(_settings.MinRoomSize + _margin, max);

        if (splitHorizontally)
        {
            node.Left = new BSPNode(new RectInt(node.Room.x, node.Room.y, node.Room.width, split));
            node.Right = new BSPNode(new RectInt(node.Room.x, node.Room.y + split, node.Room.width, node.Room.height - split));
        }
        else
        {
            node.Left = new BSPNode(new RectInt(node.Room.x, node.Room.y, split, node.Room.height));
            node.Right = new BSPNode(new RectInt(node.Room.x + split, node.Room.y, node.Room.width - split, node.Room.height));
        }

        Split(node.Left);
        Split(node.Right);
    }

    private bool DetermineSplitOrientation(BSPNode node)
    {
        bool splitHorizontally = Random.Range(0, 2) == 0;
        if (node.Room.width > node.Room.height && node.Room.width / node.Room.height >= 1.25)
        {
            splitHorizontally = false;
        }
        else if (node.Room.height > node.Room.width && node.Room.height / node.Room.width >= 1.25)
        {
            splitHorizontally = true;
        }

        return splitHorizontally;
    }

    private void CreateRooms(BSPNode node)
    {
        if (node.IsLeaf())
        {
            GenerateRoom(node);
        }
        else
        {
            CreateRooms(node.Left);
            CreateRooms(node.Right);
        }
    }

    private void GenerateRoom(BSPNode node)
    {
        int roomWidth = Random.Range(_settings.MinRoomSize, node.Room.width - 1);
        int roomHeight = Random.Range(_settings.MinRoomSize, node.Room.height - 1);
        int roomX = node.Room.x + Random.Range(0, node.Room.width - roomWidth);
        int roomY = node.Room.y + Random.Range(0, node.Room.height - roomHeight);

        RectInt room = new RectInt(roomX, roomY, roomWidth, roomHeight);
        node.GeneratedRoom = room;

        _rooms.Add(room);
        _roomCount++;

        for (int x = room.x; x < room.xMax; x++)
        {
            for (int y = room.y; y < room.yMax; y++)
            {
                _groundTilemap.SetTile(new Vector3Int(x, y, 0), _settings.GroundTile);
            }
        }
    }

    private void ConnectRooms(BSPNode node)
    {
        if (node.IsLeaf()) return;

        ConnectRooms(node.Left);
        ConnectRooms(node.Right);

        BSPNode leftRoom = GetRoom(node.Left);
        BSPNode rightRoom = GetRoom(node.Right);

        if (leftRoom.GeneratedRoom.width > 0 && rightRoom.GeneratedRoom.width > 0)
        {
            ConnectRoomNodes(node, leftRoom, rightRoom);
        }
    }

    private void ConnectRoomNodes(BSPNode node, BSPNode leftRoom, BSPNode rightRoom)
    {
        Vector3Int leftTeleportLocation = CalculateTeleportLocation(leftRoom, rightRoom);
        Vector3Int rightTeleportLocation = CalculateTeleportLocation(rightRoom, leftRoom);

        node.Left.TeleportLocations.Add(leftTeleportLocation);
        node.Right.TeleportLocations.Add(rightTeleportLocation);

        _linkTeleportDict.Add(new LinkTeleport
        {
            Node = node,
            LeftTeleportLocation = leftTeleportLocation,
            RightTeleportLocation = rightTeleportLocation,
        });
    }

    private Vector3Int CalculateTeleportLocation(BSPNode currentRoom, BSPNode targetRoom)
    {
        Vector3Int teleportLocation = new Vector3Int();
        bool locationFound = false;
        float deltaX = targetRoom.GeneratedRoom.center.x - currentRoom.GeneratedRoom.center.x;
        float deltaY = targetRoom.GeneratedRoom.center.y - currentRoom.GeneratedRoom.center.y;

        while (!locationFound)
        {
            teleportLocation = DetermineTeleportLocation(currentRoom, deltaX, deltaY);
            locationFound = !currentRoom.HasTeleportLocation(teleportLocation);
        }

        return teleportLocation;
    }

    private Vector3Int DetermineTeleportLocation(BSPNode currentRoom, float deltaX, float deltaY)
    {
        int offset = 1;
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            if (deltaX > 0)
            {
                return new Vector3Int(currentRoom.GeneratedRoom.xMax - 1 - offset, currentRoom.GeneratedRoom.y + Random.Range(offset, currentRoom.GeneratedRoom.height - offset * 2), 0);
            }
            else
            {
                return new Vector3Int(currentRoom.GeneratedRoom.x + offset, currentRoom.GeneratedRoom.y + Random.Range(offset, currentRoom.GeneratedRoom.height - offset * 2), 0);
            }
        }
        else
        {
            if (deltaY > 0)
            {
                return new Vector3Int(currentRoom.GeneratedRoom.x + Random.Range(0, currentRoom.GeneratedRoom.width - offset * 2), currentRoom.GeneratedRoom.yMax - offset, 0);
            }
            else
            {
                return new Vector3Int(currentRoom.GeneratedRoom.x + Random.Range(0, currentRoom.GeneratedRoom.width - offset * 2), currentRoom.GeneratedRoom.y + offset, 0);
            }
        }
    }

    private BSPNode GetRoom(BSPNode node)
    {
        if (node.IsLeaf())
        {
            return node;
        }

        BSPNode leftRoom = GetRoom(node.Left);
        if (leftRoom.GeneratedRoom.width > 0)
        {
            return leftRoom;
        }

        BSPNode rightRoom = GetRoom(node.Right);
        if (rightRoom.GeneratedRoom.width > 0)
        {
            return rightRoom;
        }

        return null;
    }

    public RectInt? GetRoomFromPosition(Vector3 position)
    {
        foreach (var room in _rooms)
        {
            if (room.Contains(new Vector2Int((int)position.x, (int)position.y)))
            {
                return room;
            }
        }
        return null;
    }
}
