using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private bool debugRun = false;

    [SerializeField] private GameObject[] roomPrefabs;

    [SerializeField] private GameObject[] bossRoomPrefabs;

    [Range(0f, 1f)]
    [SerializeField] private float branchProbability = 0.5f;
    [SerializeField] private int maxCorridorLength = 4;

    [Min(2)]
    [SerializeField] private int minRooms = 10;
    [Min(2)]
    [SerializeField] private int maxRooms = 20;

    private List<RoomData> graph = new List<RoomData>();

    private Dictionary<Vector2Int, GameObject> placedRooms = new();

    [SerializeField] private float roomSizeX = 50f;
    [SerializeField] private float roomSizeY = 50f;

    public Dictionary<Vector2Int, GameObject> PlacedRooms { get => placedRooms; set => placedRooms = value; }

    public static DungeonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if(debugRun)
        {
            GenerateDungeon(UnityEngine.Random.Range(0, int.MaxValue));
        }
    }

    public void GenerateDungeon(int seed)
    {
        if (minRooms > maxRooms)
        {
            throw new Exception("minRooms must be ≤ maxRooms");
        }
        UnityEngine.Random.InitState(seed);
        BuildGraph();
        InstantiateFromGraph();
    }

    /*#region BFS
    void BuildGraph()
    {
        graph.Clear();
        Queue<RoomData> queue = new Queue<RoomData>();

        RoomData start = new RoomData(Vector2Int.zero);
        graph.Add(start);
        queue.Enqueue(start);

        int targetNormals = maxRooms - 1;
        while (queue.Count > 0 && graph.Count < targetNormals)
        {
            RoomData current = queue.Dequeue();

            List<Direction> directions = new List<Direction>
            {
                Direction.North, Direction.South,
                Direction.East,  Direction.West
            };

            Shuffle(directions);

            foreach (Direction d in directions)
            {
                if (graph.Count >= targetNormals)
                {
                    break;
                }
                Vector2Int newPos = current.Position + DirectionExtensions.ToVector2Int(d);
                if (graph.Any(r => r.Position == newPos))
                {
                    continue;
                }

                current.Connections.Add(d);
                RoomData neighbour = new RoomData(newPos);
                neighbour.Connections.Add(DirectionExtensions.Opposite(d));
                graph.Add(neighbour);
                queue.Enqueue(neighbour);
            }
        }

        PlaceBossRoom(queue);
    }

    void PlaceBossRoom(Queue<RoomData> frontier)
    {
        while (frontier.Count > 0)
        {
            RoomData parent = frontier.Dequeue();

            List<Direction> freeDirections = Enum.GetValues(typeof(Direction))
                .Cast<Direction>()
                .Where(d => !parent.Connections.Contains(d))
                .Where(d => !graph.Any(r => r.Position == parent.Position + DirectionExtensions.ToVector2Int(d)))
                .ToList();

            if (freeDirections.Count == 0)
            {
                continue;
            }

            Direction pick = freeDirections[UnityEngine.Random.Range(0, freeDirections.Count)];
            parent.Connections.Add(pick);
            Vector2Int bossPos = parent.Position + DirectionExtensions.ToVector2Int(pick);
            RoomData boss = new RoomData(bossPos, boss: true);
            boss.Connections.Add(DirectionExtensions.Opposite(pick));
            graph.Add(boss);
            return;
        }

        Debug.LogError("Failed to place boss room: no free frontier.");
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    #endregion */

    #region DFS
    private void BuildGraph()
    {
        graph.Clear();

        RoomData start = new RoomData(Vector2Int.zero);
        start.IsStart = true;
        graph.Add(start);

        int minNormal = minRooms - 1;
        int steps = Mathf.Max(0, minNormal - 1);
        RoomData current = start;

        for (int i = 0; i < steps; i++)
        {
            List<Direction> freeDirs = Enum.GetValues(typeof(Direction))
                .Cast<Direction>()
                .Where(d => !current.Connections.Contains(d)
                         && !graph.Any(r => r.Position == current.Position + DirectionExtensions.ToVector2Int(d)))
                .ToList();
            if (freeDirs.Count == 0)
            {
                break;
            }

            Direction dir = freeDirs[UnityEngine.Random.Range(0, freeDirs.Count)];
            Vector2Int newPos = current.Position + DirectionExtensions.ToVector2Int(dir);

            current.Connections.Add(dir);
            RoomData next = new RoomData(newPos);
            next.Connections.Add(DirectionExtensions.Opposite(dir));
            graph.Add(next);
            current = next;
        }

        Carve(start, maxRooms - 1, 0);

        PlaceBossRoom();
    }

    private void Carve(RoomData node, int normalRoomTarget, int depth)
    {
        if (graph.Count >= normalRoomTarget)
        {
            return;
        }

        if (depth >= maxCorridorLength)
        {
            return;
        }
        
        List<Direction> directions = new List<Direction> {
            Direction.North, Direction.South,
            Direction.East,  Direction.West
        };
        Shuffle(directions);

        bool first = true;
        foreach (Direction d in directions)
        {
            if (graph.Count >= normalRoomTarget)
            {
                break;
            }

            if (!first && UnityEngine.Random.value > branchProbability)
            {
                break;
            }
            first = false;

            Vector2Int np = node.Position + DirectionExtensions.ToVector2Int(d);
            if (graph.Any(r => r.Position == np))
            {
                continue;
            }
            
            node.Connections.Add(d);
            RoomData neighbour = new RoomData(np);
            neighbour.Connections.Add(DirectionExtensions.Opposite(d));
            graph.Add(neighbour);
            Carve(neighbour, normalRoomTarget, depth + 1);
        }
    }

    void PlaceBossRoom()
    {
        List<RoomData> leaves = graph
          .Where(r => !r.IsBoss && r.Connections.Count < 4)
          .OrderBy(_ => UnityEngine.Random.value)
          .ToList();

        foreach (RoomData parent in leaves)
        {
            List<Direction> freeDirections = Enum.GetValues(typeof(Direction))
                .Cast<Direction>()
                .Where(d => !parent.Connections.Contains(d)
                         && !graph.Any(r => r.Position == parent.Position + DirectionExtensions.ToVector2Int(d)))
                .ToList();

            if (freeDirections.Count == 0)
            {
                continue;
            }

            Direction dir = freeDirections[UnityEngine.Random.Range(0, freeDirections.Count)];
            parent.Connections.Add(dir);

            Vector2Int bossPos = parent.Position + DirectionExtensions.ToVector2Int(dir);
            RoomData bossRoom = new RoomData(bossPos, boss: true);
            bossRoom.Connections.Add(DirectionExtensions.Opposite(dir));
            graph.Add(bossRoom);
            return;
        }

        Debug.LogError("Failed to place boss room: no leaf had a free exit.");
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    #endregion

    void InstantiateFromGraph()
    {
        placedRooms.Clear();

        foreach (RoomData data in graph)
        {
            GameObject[] roomPool = data.IsBoss ? bossRoomPrefabs : roomPrefabs;

            string signature = string.Concat(
                data.Connections
                    .OrderBy(d => d)
                    .Select(d => DirectionExtensions.ToShortString(d))
            );

            GameObject prefab = roomPool.FirstOrDefault(p => p.name.EndsWith($"_{signature}"));
            if (prefab == null)
            {
                Debug.LogError($"No prefab matches signature '{signature}'");
                continue;
            }

            Vector3 worldPos = new Vector3(data.Position.x * roomSizeX, 0, data.Position.y * roomSizeY);
            GameObject go = Instantiate(prefab, worldPos, Quaternion.identity);
            go.GetComponent<RoomController>().Init(data);
            placedRooms[data.Position] = go;
        }
    }

    public RoomData GetRoomData(Vector2Int pos) => graph.FirstOrDefault(r => r.Position == pos);
}
