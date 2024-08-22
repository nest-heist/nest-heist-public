using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PerlinNoiseMapSettings
{
    // public Tile TilePlains;
    // public Tile TileHills;
    // public Tile TileMountains;
    public GameObject TreePrefab;
    public TileBase DecoTileBase;

    [Range(0.0f, 1.0f)]
    public float ForestThreshold = 0.4f;

    [Range(4.0f, 20.0f)]
    public float Magnification = 7.0f;
}

[CreateAssetMenu(fileName = "DungeonMapSettingsSO", menuName = "Settings/DungeonMapSettingsSO")]
public class DungeonMapSettingsSO : ScriptableObject
{
    public TileBase GroundTile;
    public TileBase RoundGroundTile;
    public GameObject TreePrefab;
    public GameObject TeleportPrefab;
    public int MinRoomSize = 5;
    public int Padding = 20;
    public PerlinNoiseMapSettings PerlinNoiseMapSettings;
}
