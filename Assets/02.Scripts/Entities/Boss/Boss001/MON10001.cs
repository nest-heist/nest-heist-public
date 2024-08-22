using System.Collections.Generic;
using UnityEngine;

public class MON10001 : Boss
{
    [Header("Room Pos")]
    public Vector2 TopLeft;
    public Vector2 TopRight;
    public Vector2 BottomLeft;
    public Vector2 BottomRight;

    public List<Stone> Stones;

    protected override void Start()
    {
        // 타일맵의 바운딩 박스를 가져옵니다.
        BoundsInt bounds = Room.cellBounds;

        // 타일맵의 각 꼭지점 셀 위치를 계산합니다.
        Vector3Int bottomLeftCell = new Vector3Int(bounds.xMin, bounds.yMin, 0);
        Vector3Int bottomRightCell = new Vector3Int(bounds.xMax - 1, bounds.yMin, 0);
        Vector3Int topLeftCell = new Vector3Int(bounds.xMin, bounds.yMax - 1, 0);
        Vector3Int topRightCell = new Vector3Int(bounds.xMax - 1, bounds.yMax - 1, 0);

        // 각 꼭지점 셀 위치를 월드 좌표로 변환합니다.
        BottomLeft = Room.CellToWorld(bottomLeftCell);
        BottomRight = Room.CellToWorld(bottomRightCell) + new Vector3(Room.cellSize.x, 0, 0);
        TopLeft = Room.CellToWorld(topLeftCell) + new Vector3(0, Room.cellSize.y, 0);
        TopRight = Room.CellToWorld(topRightCell) + new Vector3(Room.cellSize.x, Room.cellSize.y, 0);
    }
}
