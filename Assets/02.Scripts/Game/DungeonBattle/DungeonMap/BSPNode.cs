using System;
using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
  public RectInt Room;
  public BSPNode Left;
  public BSPNode Right;
  public RectInt GeneratedRoom; // 생성된(타일맵이 생성된) 방 정보 저장
  public List<Vector3Int> TeleportLocations = new List<Vector3Int>();

  public BSPNode(RectInt room)
  {
    Room = room;
  }

  public bool IsLeaf()
  {
    return Left == null && Right == null;
  }

  public bool HasTeleportLocation(Vector3Int location)
  {
    foreach (Vector3Int teleportLocation in TeleportLocations)
    {
      if (teleportLocation == location)
      {
        return true;
      }
    }
    return false;
  }
}
