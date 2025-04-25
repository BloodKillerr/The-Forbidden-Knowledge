using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomData
{
    private Vector2Int position;
    private List<Direction> connections = new List<Direction>();
    private bool isBoss = false;
    [NonSerialized] private bool hasBeenEntered = false;

    public RoomData(Vector2Int pos, bool boss = false)
    {
        position = pos;
        isBoss = boss;
    }

    public Vector2Int Position { get => position; set => position = value; }
    public List<Direction> Connections { get => connections; set => connections = value; }
    public bool IsBoss { get => isBoss; set => isBoss = value; }
    public bool HasBeenEntered { get => hasBeenEntered; set => hasBeenEntered = value; }
}
