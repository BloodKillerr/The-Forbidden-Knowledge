using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataApplier : MonoBehaviour
{
    private void Start()
    {
        MasterSaveData data = SaveManager.LoadedData;
        if (data == null)
        {
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex != data.LastSceneBuildIndex)
        {
            Debug.LogWarning($"[GameDataApplier] Scene index mismatch! " +
                             $"Expected {data.LastSceneBuildIndex}, but we're in {currentIndex}.");
        }

        LoadPlayerData(data);
        LoadPlayerStatsData(data);
        LoadLevelingData(data);
        LoadInventoryData(data);
        LoadEquipmentData(data);
        LoadSpellData(data);
        LoadConsumableData(data);
        LoadNotesData(data);
        LoadAbilitiesData(data);
        LoadDungeonData(data);

        UIManager.Instance.ClearMessages();

        SaveManager.IsLoadingSave = false;
        SaveManager.LoadedData = null;
    }

    private void LoadPlayerData(MasterSaveData data)
    {
        if (data.PlayerData != null)
        {
            GameObject playerObj = Player.Instance?.gameObject;
            if (playerObj != null)
            {
                playerObj.transform.position = data.PlayerData.position;
                playerObj.transform.rotation = Quaternion.Euler(data.PlayerData.rotationEuler);
            }
            else
            {
                Debug.LogError("[GameDataApplier] Could not find Player.Instance to apply saved data.");
            }
        }
    }

    private void LoadInventoryData(MasterSaveData data)
    {
        if (data.inventoryData != null && InventoryManager.Instance != null)
        {
            InventoryManager invMgr = InventoryManager.Instance;
            invMgr.Items.Clear();

            foreach (InventoryItemEntry entry in data.inventoryData.items)
            {
                if (string.IsNullOrEmpty(entry.itemName))
                    continue;

                Item itemPrefab = ItemDatabase.Instance.GetByName(entry.itemName);
                if (itemPrefab != null)
                {
                    Item newItem = Instantiate(itemPrefab);
                    newItem.Amount = entry.amount;
                    invMgr.Add(newItem);
                }
                else
                {
                    Debug.LogWarning($"[GameDataApplier] Inventory: Missing Item '{entry.itemName}' in database.");
                }
            }
        }
    }

    private void LoadEquipmentData(MasterSaveData data)
    {
        if (data.equipmentData != null
            && EquipmentManager.Instance != null
            && InventoryManager.Instance != null)
        {
            EquipmentManager eqMgr = EquipmentManager.Instance;
            eqMgr.UnEquipAll();

            int numSlots = data.equipmentData.equippedBySlot.Length;
            for (int slotIndex = 0; slotIndex < numSlots; slotIndex++)
            {
                string eqName = data.equipmentData.equippedBySlot[slotIndex];
                if (string.IsNullOrEmpty(eqName)) continue;

                Equipment toEquip = null;
                foreach (Item invItem in InventoryManager.Instance.Items)
                {
                    if (invItem is Equipment e && e.Name == eqName)
                    {
                        toEquip = e;
                        break;
                    }
                }

                if (toEquip == null)
                {
                    Equipment fallback = EquipmentDatabase.Instance.GetByName(eqName);
                    if (fallback != null)
                    {
                        toEquip = Instantiate(fallback);
                    }
                    else
                    {
                        Debug.LogWarning($"[GameDataApplier] Equipment '{eqName}' not in bag or DB.");
                        continue;
                    }
                }

                eqMgr.Equip(toEquip);
                UIManager.Instance.UpdateEquipmentSlotHolder(toEquip, true);
            }
        }
    }

    private void LoadSpellData(MasterSaveData data)
    {
        if (data.spellData != null
    && SpellManager.Instance != null
    && InventoryManager.Instance != null)
        {
            SpellManager spMgr = SpellManager.Instance;
            spMgr.UnEquipAll();

            if (!string.IsNullOrEmpty(data.spellData.spell1Name))
            {
                float savedRem1 = data.spellData.spell1RemainingCooldown;
                Debug.Log($"[GameDataApplier] Restoring Spell1 '{data.spellData.spell1Name}' " +
                          $"with remainingCd={savedRem1:F2}s");

                Spell toEquip = null;
                foreach (Item invItem in InventoryManager.Instance.Items)
                {
                    if (invItem is Spell s && s.Name == data.spellData.spell1Name)
                    {
                        toEquip = s;
                        break;
                    }
                }
                if (toEquip == null)
                {
                    Spell fallback = SpellDatabase.Instance.GetByName(data.spellData.spell1Name);
                    if (fallback != null) toEquip = Instantiate(fallback);
                    else Debug.LogWarning($"[GameDataApplier] Spell '{data.spellData.spell1Name}' not in bag or DB.");
                }

                if (toEquip != null)
                {
                    spMgr.EquipSpell(toEquip);
                    UIManager.Instance.UpdateEquipmentSlotHolder(toEquip, true);

                    spMgr.SetRemainingCooldown(1, savedRem1);

                    UIManager.Instance.ResumeSpellCooldown(1, savedRem1);
                }
            }

            if (!string.IsNullOrEmpty(data.spellData.spell2Name))
            {
                float savedRem2 = data.spellData.spell2RemainingCooldown;
                Debug.Log($"[GameDataApplier] Restoring Spell2 '{data.spellData.spell2Name}' " +
                          $"with remainingCd={savedRem2:F2}s");

                Spell toEquip = null;
                foreach (Item invItem in InventoryManager.Instance.Items)
                {
                    if (invItem is Spell s && s.Name == data.spellData.spell2Name)
                    {
                        toEquip = s;
                        break;
                    }
                }
                if (toEquip == null)
                {
                    Spell fallback = SpellDatabase.Instance.GetByName(data.spellData.spell2Name);
                    if (fallback != null) toEquip = Instantiate(fallback);
                    else Debug.LogWarning($"[GameDataApplier] Spell '{data.spellData.spell2Name}' not in bag or DB.");
                }

                if (toEquip != null)
                {
                    spMgr.EquipSpell(toEquip);
                    UIManager.Instance.UpdateEquipmentSlotHolder(toEquip, true);

                    spMgr.SetRemainingCooldown(2, savedRem2);
                    UIManager.Instance.ResumeSpellCooldown(2, savedRem2);
                }
            }
        }
    }

    private void LoadConsumableData(MasterSaveData data)
    {
        if (data.consumableData != null
    && ConsumableManager.Instance != null
    && InventoryManager.Instance != null)
        {
            ConsumableManager cMgr = ConsumableManager.Instance;
            cMgr.UnEquipAll();

            if (!string.IsNullOrEmpty(data.consumableData.consumable1Name))
            {
                float savedRem1 = data.consumableData.consumable1RemainingCooldown;
                Debug.Log($"[GameDataApplier] Restoring Cons1 '{data.consumableData.consumable1Name}' " +
                          $"with remainingCd={savedRem1:F2}s");

                Consumable toEquip = null;
                foreach (Item invItem in InventoryManager.Instance.Items)
                {
                    if (invItem is Consumable c && c.Name == data.consumableData.consumable1Name)
                    {
                        toEquip = c;
                        break;
                    }
                }
                if (toEquip == null)
                {
                    Consumable fallback = ConsumableDatabase.Instance.GetByName(data.consumableData.consumable1Name);
                    if (fallback != null) toEquip = Instantiate(fallback);
                    else Debug.LogWarning($"[GameDataApplier] Consumable '{data.consumableData.consumable1Name}' not in bag or DB.");
                }

                if (toEquip != null)
                {
                    cMgr.EquipConsumable(toEquip);
                    UIManager.Instance.UpdateEquipmentSlotHolder(toEquip, true);

                    cMgr.SetRemainingCooldown(1, savedRem1);
                    UIManager.Instance.ResumeConsumableCooldown(1, savedRem1);
                }
            }

            if (!string.IsNullOrEmpty(data.consumableData.consumable2Name))
            {
                float savedRem2 = data.consumableData.consumable2RemainingCooldown;
                Debug.Log($"[GameDataApplier] Restoring Cons2 '{data.consumableData.consumable2Name}' " +
                          $"with remainingCd={savedRem2:F2}s");

                Consumable toEquip = null;
                foreach (Item invItem in InventoryManager.Instance.Items)
                {
                    if (invItem is Consumable c && c.Name == data.consumableData.consumable2Name)
                    {
                        toEquip = c;
                        break;
                    }
                }
                if (toEquip == null)
                {
                    Consumable fallback = ConsumableDatabase.Instance.GetByName(data.consumableData.consumable2Name);
                    if (fallback != null) toEquip = Instantiate(fallback);
                    else Debug.LogWarning($"[GameDataApplier] Consumable '{data.consumableData.consumable2Name}' not in bag or DB.");
                }

                if (toEquip != null)
                {
                    cMgr.EquipConsumable(toEquip);
                    UIManager.Instance.UpdateEquipmentSlotHolder(toEquip, true);

                    cMgr.SetRemainingCooldown(2, savedRem2);
                    UIManager.Instance.ResumeConsumableCooldown(2, savedRem2);
                }
            }
        }
    }

    private void LoadNotesData(MasterSaveData data)
    {
        if (data.notesData != null && NotesManager.Instance != null)
        {
            NotesManager.Instance.SetUnlockedNoteIDs(data.notesData.unlockedNoteIDs);
        }
    }

    private void LoadPlayerStatsData(MasterSaveData data)
    {
        if (data.statsData != null && Player.Instance != null)
        {
            Player.Instance.GetComponent<PlayerStats>().ApplyStatsData(data.statsData);
        }
    }

    private void LoadLevelingData(MasterSaveData data)
    {
        if (data.levelingData != null && LevelingManager.Instance != null)
        {
            LevelingManager.Instance.ApplyLevelingData(data.levelingData);
        }
    }

    private void LoadAbilitiesData(MasterSaveData data)
    {
        if (data.abilitiesData != null && AbilityManager.Instance != null)
        {
            AbilityManager.Instance.SetCurrentAbilitiesByName(data.abilitiesData.abilityNames);
        }
    }

    private void LoadDungeonData(MasterSaveData data)
    {
        if (data.dungeonData != null && DungeonManager.Instance != null)
        {
            DungeonManager.Instance.RestoreDungeonState(data.dungeonData);
        }
    }
}
