﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
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

    public GameObject ChestPrefab;

    public List<Item> ItemsToDrop = new List<Item>();

    public GameObject portalPrefab;

    private int seed;

    public Dictionary<Vector2Int, GameObject> PlacedRooms { get => placedRooms; set => placedRooms = value; }

    public static DungeonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (!SaveManager.IsLoadingSave)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
            GenerateDungeon(seed);
            Debug.Log($"[DEBUG] Generated dungeon seed={seed}");
        }
    }

    public void GenerateDungeon(int seed)
    {
        if (minRooms > maxRooms)
        {
            throw new Exception("minRooms must be ≤ maxRooms");
        }

        Player.Instance.GetComponent<PlayerTracker>().EnterDungeon();

        MinimapManager.Instance?.ClearMinimap();

        UnityEngine.Random.InitState(seed);
        BuildGraph();
        InstantiateFromGraph();

        if(!SaveManager.IsLoadingSave)
        {
            Player.Instance.gameObject.transform.position = Vector3.zero;
        }

        MinimapManager.Instance?.CreateRoomIcon(Vector2Int.zero);
        MinimapManager.Instance?.HighlightPlayer(Vector2Int.zero);
    }

    public Vector2Int GetRoomPositionFromWorld(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / 25f);
        int y = Mathf.RoundToInt(worldPos.z / 25f);
        return new Vector2Int(x, y);
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
        Dictionary<RoomData, int> distances = new Dictionary<RoomData, int>();
        Queue<RoomData> queue = new Queue<RoomData>();

        RoomData start = graph.First(r => r.Position == Vector2Int.zero);
        distances[start] = 0;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            RoomData node = queue.Dequeue();
            int distance = distances[node];
            foreach (Direction direction in node.Connections)
            {
                Vector2Int neighborPos = node.Position + DirectionExtensions.ToVector2Int(direction);
                RoomData neighbor = graph.First(r => r.Position == neighborPos);
                if (distances.ContainsKey(neighbor))
                {
                    continue;
                }
                distances[neighbor] = distance + 1;
                queue.Enqueue(neighbor);
            }
        }

        List<RoomData> leaves = graph
            .Where(r => !r.IsBoss
                        && !(r.Position == Vector2Int.zero)
                        && r.Connections.Count == 1)
            .ToList();

        if (leaves.Count == 0)
        {
            return;
        }

        RoomData farthestLeaf = leaves
            .OrderByDescending(r => distances[r])
            .First();

        List<Direction> freeDirections = Enum.GetValues(typeof(Direction))
            .Cast<Direction>()
            .Where(direction => !farthestLeaf.Connections.Contains(direction)
                        && !graph.Any(r => r.Position == farthestLeaf.Position + DirectionExtensions.ToVector2Int(direction)))
            .ToList();

        if (freeDirections.Count == 0)
        {
            return;
        }

        Direction chosenDirection = freeDirections[UnityEngine.Random.Range(0, freeDirections.Count)];
        farthestLeaf.Connections.Add(chosenDirection);

        Vector2Int bossPos = farthestLeaf.Position + DirectionExtensions.ToVector2Int(chosenDirection);
        RoomData bossRoom = new RoomData(bossPos, boss: true);
        bossRoom.Connections.Add(DirectionExtensions.Opposite(chosenDirection));
        graph.Add(bossRoom);
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

    public DungeonData CollectDungeonState()
    {
        DungeonData d = new DungeonData();

        d.seed = seed;

        Vector2Int playerRoom = Player.Instance.GetComponent<PlayerTracker>().CurrentRoomPos;
        d.playerRoomPosition = playerRoom;
        d.playerWorldPosition = Player.Instance.transform.position;

        d.roomsEntered = graph
            .Where(r => r.HasBeenEntered)
            .Select(r => r.Position)
            .ToList();

        d.roomsCleared = new List<Vector2Int>();
        foreach (Vector2Int pos in d.roomsEntered)
        {
            if (!placedRooms.TryGetValue(pos, out var roomGo) || roomGo == null)
                continue;

            RoomController rc = roomGo.GetComponent<RoomController>();
            if (rc?.Enemies != null && rc.Enemies.Count == 0)
                d.roomsCleared.Add(pos);
        }

        d.roomsState = new List<RoomControllerState>();
        foreach (var kv in placedRooms)
        {
            Vector2Int pos = kv.Key;
            GameObject go = kv.Value;
            RoomController rc = go.GetComponent<RoomController>();
            if (rc == null) continue;

            d.roomsState.Add(new RoomControllerState
            {
                position = pos,
                hasBeenEntered = rc.Data.HasBeenEntered,
                isFinished = rc.IsFinished,
                checkCompletion = rc.CheckCompletion
            });
        }

        d.minimapVisited = MinimapManager.Instance.GetVisitedRoomPositions();

        d.currentRoomState = new EnemyRoomState
        {
            roomPosition = playerRoom,
            enemies = placedRooms.TryGetValue(playerRoom, out var curGo) && curGo != null
                ? curGo.GetComponent<RoomController>()
                       .Enemies
                       .Where(e => e != null)
                       .Select(e => new EnemyState
                       {
                           enemyName = e.GetComponent<EnemyStats>().CharacterName,
                           position = e.transform.position,
                           remainingHP = e.GetComponent<EnemyStats>().CurrentHealth
                       })
                       .ToList()
                : new List<EnemyState>()
        };

        d.allChests = new List<ChestState>();
        foreach (var kv in placedRooms)
        {
            foreach (Chest chest in kv.Value.GetComponentsInChildren<Chest>())
            {
                List<LootData> lootData = chest.LootTable
                    .Select(ld => new LootData
                    {
                        itemName = ld.Item.Name,
                        minAmount = ld.MinAmount,
                        maxAmount = ld.MaxAmount,
                        dropChance = ld.DropChance
                    })
                    .ToList();

                d.allChests.Add(new ChestState
                {
                    position = chest.transform.position,
                    lootTable = lootData
                });
            }
        }

        Boss boss = FindAnyObjectByType<Boss>();
        d.bossKilled = boss == null || boss.GetComponent<EnemyStats>().CurrentHealth <= 0;

        BossPortal portal = FindAnyObjectByType<BossPortal>();
        if (portal != null)
        {
            d.portalData = new BossPortalData
            {
                position = portal.transform.position,
                rotation = portal.transform.rotation,
                sceneIndex = portal.SceneIndex,
                isFinal = portal.isFinal
            };
        }
        else
        {
            d.portalData = null;
        }

        return d;
    }

    public void RestoreDungeonState(DungeonData d)
    {
        seed = d.seed;
        GenerateDungeon(d.seed);

        foreach (RoomControllerState rs in d.roomsState)
        {
            if (rs.hasBeenEntered && placedRooms.TryGetValue(rs.position, out var roomGo) && roomGo != null)
            {
                EnemySpawner spawner = roomGo.GetComponent<EnemySpawner>();
                if (spawner != null) spawner.enabled = false;

                RoomController rc = roomGo.GetComponent<RoomController>();
                if (rc != null)
                    Enemy.OnEnemyKilled.RemoveListener(rc.RemoveEnemy);
            }
        }

        Vector2Int curPos = d.currentRoomState.roomPosition;

        foreach (RoomControllerState rs in d.roomsState)
        {
            if (rs.position == curPos) continue;

            RoomData roomData = GetRoomData(rs.position);
            if (roomData != null)
                roomData.HasBeenEntered = rs.hasBeenEntered;

            if (placedRooms.TryGetValue(rs.position, out var roomGo) && roomGo != null)
            {
                RoomController rc = roomGo.GetComponent<RoomController>();
                if (rc != null)
                {
                    rc.IsFinished = rs.isFinished;
                    rc.CheckCompletion = rs.checkCompletion;
                }
            }
        }

        MinimapManager.Instance.SetVisitedRoomPositions(d.minimapVisited);
        MinimapManager.Instance.HighlightPlayer(d.playerRoomPosition);

        Rigidbody rb = Player.Instance.GetComponent<PlayerMovement>().Rb;
        rb.position = d.playerWorldPosition;

        if (placedRooms.TryGetValue(curPos, out var curRoomGo) && curRoomGo != null)
        {
            RoomController rc = curRoomGo.GetComponent<RoomController>();
            if (rc != null)
            {
                if (rc.Enemies != null)
                {
                    foreach (Enemy e in rc.Enemies.ToList())
                        if (e != null) Destroy(e.gameObject);
                    rc.Enemies.Clear();
                }

                foreach (EnemyState es in d.currentRoomState.enemies)
                {
                    GameObject prefab = Resources.Load<GameObject>($"Enemies/{es.enemyName}");
                    if (prefab == null) continue;
                    GameObject go = Instantiate(prefab, es.position, Quaternion.identity, rc.transform);
                    Enemy enemy = go.GetComponent<Enemy>();
                    enemy.GetComponent<EnemyStats>().CurrentHealth = es.remainingHP;
                    enemy.RoomController = rc;

                    Enemy.OnEnemyKilled.AddListener(rc.RemoveEnemy);

                    rc.Enemies.Add(enemy);
                    enemy.EnemyStats.CheckHealth();
                }

                rc.CheckCompletion = true;

                bool wasCleared = d.roomsCleared != null && d.roomsCleared.Contains(curPos);
                rc.IsFinished = wasCleared;
            }
        }

        foreach (var kv in placedRooms)
            foreach (Chest chest in kv.Value.GetComponentsInChildren<Chest>())
                Destroy(chest.gameObject);

        foreach (ChestState cs in d.allChests)
        {
            GameObject go = Instantiate(ChestPrefab, cs.position, Quaternion.identity);
            Chest ch = go.GetComponent<Chest>();
            ch.LootTable.Clear();
            foreach (LootData ld in cs.lootTable)
            {
                Item item = ItemDatabase.Instance.GetByName(ld.itemName);
                ch.AddItem(item, ld.minAmount, ld.maxAmount, ld.dropChance);
            }
        }

        if (d.bossKilled)
        {
            Boss bossObj = FindAnyObjectByType<Boss>();
            if (bossObj != null) Destroy(bossObj.gameObject);
        }
        if (d.portalData.sceneIndex != 0)
        {
            GameObject go = Instantiate(
                portalPrefab,
                d.portalData.position,
                d.portalData.rotation
            );
            BossPortal portal = go.GetComponent<BossPortal>();
            portal.SceneIndex = d.portalData.sceneIndex;
            portal.isFinal = d.portalData.isFinal;
        }
    }

    public RoomData GetRoomData(Vector2Int pos) => graph.FirstOrDefault(r => r.Position == pos);
}
