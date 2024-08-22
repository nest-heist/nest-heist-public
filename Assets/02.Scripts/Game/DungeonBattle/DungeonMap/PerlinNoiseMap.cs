using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinNoiseMap
{
    private PerlinNoiseMapSettings _settings;
    private float _randomOffsetX;
    private float _randomOffsetY;
    private Tilemap _groundTilemap;
    private Tilemap _groundNoneSortTilemap;
    private GameObject _tilemapGameObject;

    public PerlinNoiseMap(PerlinNoiseMapSettings settings, Transform parentTransform)
    {
        _settings = settings;
        _randomOffsetX = Random.Range(0f, 10000f);
        _randomOffsetY = Random.Range(0f, 10000f);
        CreateTilemap(parentTransform);
    }

    private void CreateTilemap(Transform parentTransform)
    {
        _tilemapGameObject = new GameObject("NoiseTilemap");
        _tilemapGameObject.transform.SetParent(parentTransform, false);
        var tilemapComponent = _tilemapGameObject.AddComponent<Tilemap>();
        var tilemapRenderer = _tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
        _groundTilemap = tilemapComponent;

        _tilemapGameObject = new GameObject("NoneSortNoiseTilemap");
        _tilemapGameObject.transform.SetParent(parentTransform, false);
        var tilemapComponent2 = _tilemapGameObject.AddComponent<Tilemap>();
        var tilemapRenderer2 = _tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer2.mode = TilemapRenderer.Mode.Individual;
        tilemapRenderer2.sortingOrder = -1;
        _groundNoneSortTilemap = tilemapComponent2;

    }

    public void GenerateMap(RectInt room)
    {
        int roomWidth = room.width;
        int roomHeight = room.height - 1;
        int xOffset = room.x;
        int yOffset = room.y + 1;

        List<List<int>> noiseGrid = new List<List<int>>();

        Dictionary<int, TileBase> tileset = CreateTileset();
        GenerateMapTiles(roomWidth, roomHeight, xOffset, yOffset, tileset, noiseGrid);
    }

    private Dictionary<int, TileBase> CreateTileset()
    {
        return new Dictionary<int, TileBase>
        {
            {0, new Tile()},
            {2, new Tile()},
            {3, _settings.DecoTileBase}
        };
    }

    private void GenerateMapTiles(int roomWidth, int roomHeight, int xOffset, int yOffset, Dictionary<int, TileBase> tileset, List<List<int>> noiseGrid)
    {
        for (int x = 0; x < roomWidth; x++)
        {
            noiseGrid.Add(new List<int>());

            for (int y = 0; y < roomHeight; y++)
            {
                int tileId = GetIdUsingPerlin(x + xOffset, y + yOffset, tileset.Count);
                noiseGrid[x].Add(tileId);
                CreateTile(tileId, x + xOffset, y + yOffset, tileset);
            }
        }
    }

    private int GetIdUsingPerlin(int x, int y, int tilesetCount)
    {
        float rawPerlin = Mathf.PerlinNoise(
            (x + _randomOffsetX) / _settings.Magnification,
            (y + _randomOffsetY) / _settings.Magnification
        );
        float clampPerlin = Mathf.Clamp01(rawPerlin);
        float scaledPerlin = clampPerlin * (tilesetCount + 1);

        if (scaledPerlin == (tilesetCount + 1))
        {
            scaledPerlin = tilesetCount;
        }

        int tileId = Mathf.FloorToInt(scaledPerlin);

        if (tileId == 1 && clampPerlin < _settings.ForestThreshold)
        {
            tileId = 0;
        }

        return tileId;
    }

    private void CreateTile(int tileId, int x, int y, Dictionary<int, TileBase> tileset)
    {
        if (tileId == 1)
        {
            Tile treeTile = ScriptableObject.CreateInstance<Tile>();
            treeTile.gameObject = _settings.TreePrefab;
            _groundTilemap.SetTile(new Vector3Int(x, y, 0), treeTile);
        }
        else
        {
            TileBase tile = tileset[tileId];
            Vector3Int tilePosition = new Vector3Int(x, y, 0);
            _groundNoneSortTilemap.SetTile(tilePosition, tile);
        }
    }
}
