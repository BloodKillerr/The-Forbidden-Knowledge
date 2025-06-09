using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MasterSaveData
{
    public int LastSceneBuildIndex;

    public PlayerData PlayerData;

    public InventoryData inventoryData;

    public EquipmentData equipmentData;

    public SpellData spellData;

    public ConsumableData consumableData;

    public NotesData notesData;

    public StatsData statsData;

    public LevelingData levelingData;

    public AbilitiesData abilitiesData;

    public DungeonData dungeonData;
}

[Serializable]
public class PlayerData
{
    public Vector3 position;
    public Vector3 rotationEuler;

    public PlayerData() { }

    public PlayerData(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotationEuler = rot.eulerAngles;
    }
}

[Serializable]
public class InventoryData
{
    public List<InventoryItemEntry> items = new List<InventoryItemEntry>();
}

[Serializable]
public class InventoryItemEntry
{
    public string itemName;
    public int amount;

    public InventoryItemEntry() { }
    public InventoryItemEntry(string name, int amt)
    {
        itemName = name;
        amount = amt;
    }
}

[Serializable]
public class EquipmentData
{
    public string[] equippedBySlot;

    public EquipmentData() { }

    public EquipmentData(int numSlots)
    {
        equippedBySlot = new string[numSlots];
        for (int i = 0; i < numSlots; i++)
        {
            equippedBySlot[i] = string.Empty;
        }
    }
}

[Serializable]
public class SpellData
{
    public string spell1Name;
    public string spell2Name;

    public float spell1RemainingCooldown;
    public float spell2RemainingCooldown;
}

[Serializable]
public class ConsumableData
{
    public string consumable1Name;
    public string consumable2Name;

    public float consumable1RemainingCooldown;
    public float consumable2RemainingCooldown;
}

[Serializable]
public class NotesData
{
    public List<string> unlockedNoteIDs = new List<string>();
}

[Serializable]
public class StatsData
{
    public int currentHealth;
    public int baseMaxHealth;

    public bool isInvincible;

    public int baseArmor;
    public int baseDamage;
    public int baseMovementSpeed;

    public int currentDodgeCharges;
    public int baseMaxDodgeCharges;
}

[Serializable]
public class LevelingData
{
    public int statPoints;
    public int currentXP;
    public int requiredXP;
}

[Serializable]
public class AbilitiesData
{
    public List<string> abilityNames = new List<string>();
}

[Serializable]
public class DungeonData
{
    public int seed;
    public Vector2Int playerRoomPosition;
    public Vector3 playerWorldPosition;
    public List<Vector2Int> roomsEntered;
    public List<Vector2Int> roomsCleared;
    public List<RoomControllerState> roomsState;
    public List<Vector2Int> minimapVisited;
    public EnemyRoomState currentRoomState;
    public List<ChestState> allChests;
    public bool bossKilled;
    public BossPortalData portalData;
}

[Serializable]
public class EnemyRoomState
{
    public Vector2Int roomPosition;
    public List<EnemyState> enemies;
}

[Serializable]
public class EnemyState
{
    public string enemyName;
    public Vector3 position;
    public int remainingHP;
}

[Serializable]
public class ChestState
{
    public Vector3 position;
    public List<LootData> lootTable;
}

[Serializable]
public class LootData
{
    public string itemName;
    public int minAmount;
    public int maxAmount;
    public float dropChance;
}

[Serializable]
public class RoomControllerState
{
    public Vector2Int position;
    public bool hasBeenEntered;
    public bool isFinished;
    public bool checkCompletion;
}

[Serializable]
public class BossPortalData
{
    public Vector3 position;
    public Quaternion rotation;
    public int sceneIndex;
    public bool isFinal;
}