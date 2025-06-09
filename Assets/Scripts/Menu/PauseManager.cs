using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	public static PauseManager Instance { get; private set; }

	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject optionsPanel;
	[SerializeField] private GameObject notesPanel;
	[SerializeField] private GameObject controlsPanel;
	public GameObject RebindingUI;

	public GameObject ResumeButton;
	public GameObject OptionsButton;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	public void Resume()
	{
		Debug.Log("Resume");
        UIManager.Instance.ToogleMenu(MenuType.PAUSE);
    }

	public void Save()
	{
        MasterSaveData data = new MasterSaveData();

        data.LastSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

		SavePlayerData(data);
        SaveInventoryData(data);
        SaveEquipmentData(data);
        SaveSpellData(data);
        SaveConsumableData(data);
        SaveNotesData(data);
        SavePlayerStatsData(data);
        SaveLevelingData(data);
        SaveAbilitiesData(data);
        SaveDungeonData(data);

        SaveManager.SaveGame(data);

		UIManager.Instance.OnGameSaved();
        Debug.Log("Save");
	}

	private void SavePlayerData(MasterSaveData data)
	{
        GameObject playerObject = Player.Instance?.gameObject;
        if (playerObject == null)
        {
            Debug.LogError("[PauseMenu] No Player instance found to save position/rotation.");
        }
        else
        {
            Vector3 pos = playerObject.transform.position;
            Quaternion rot = playerObject.transform.rotation;
            data.PlayerData = new PlayerData(pos, rot);
        }
    }

	private void SaveInventoryData(MasterSaveData data)
	{
        data.inventoryData = new InventoryData();
        if (InventoryManager.Instance != null)
        {
            foreach (Item invItem in InventoryManager.Instance.Items)
            {
                data.inventoryData.items.Add(
                    new InventoryItemEntry(invItem.Name, invItem.Amount)
                );
            }
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No InventoryManager found—saving empty inventory.");
        }
    }

	private void SaveEquipmentData(MasterSaveData data)
	{
        if (EquipmentManager.Instance != null)
        {
            int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
            data.equipmentData = new EquipmentData(numSlots);

            for (int slotIndex = 0; slotIndex < numSlots; slotIndex++)
            {
                Equipment equipped = EquipmentManager.Instance.CurrentEquipment[slotIndex];
                data.equipmentData.equippedBySlot[slotIndex] =
                    (equipped != null) ? equipped.Name : string.Empty;
            }
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No EquipmentManager found—saving no equipment.");
        }
    }

	private void SaveSpellData(MasterSaveData data)
	{
        data.spellData = new SpellData();
        if (SpellManager.Instance != null)
        {
            data.spellData.spell1Name = (SpellManager.Instance.Spell1 != null)
                ? SpellManager.Instance.Spell1.Name
                : string.Empty;
            data.spellData.spell2Name = (SpellManager.Instance.Spell2 != null)
                ? SpellManager.Instance.Spell2.Name
                : string.Empty;

            data.spellData.spell1RemainingCooldown = SpellManager.Instance.GetRemainingCooldown(1);
            data.spellData.spell2RemainingCooldown = SpellManager.Instance.GetRemainingCooldown(2);
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No SpellManager found—saving no spells.");
        }
    }

	private void SaveConsumableData(MasterSaveData data)
	{
        data.consumableData = new ConsumableData();
        if (ConsumableManager.Instance != null)
        {
            data.consumableData.consumable1Name = (ConsumableManager.Instance.Consumable1 != null)
                ? ConsumableManager.Instance.Consumable1.Name
                : string.Empty;
            data.consumableData.consumable2Name = (ConsumableManager.Instance.Consumable2 != null)
                ? ConsumableManager.Instance.Consumable2.Name
                : string.Empty;

            data.consumableData.consumable1RemainingCooldown =
                ConsumableManager.Instance.GetRemainingCooldown(1);
            data.consumableData.consumable2RemainingCooldown =
                ConsumableManager.Instance.GetRemainingCooldown(2);
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No ConsumableManager found—saving no consumables.");
        }
    }

    private void SaveNotesData(MasterSaveData data)
    {
        data.notesData = new NotesData();
        if (NotesManager.Instance != null)
        {
            data.notesData.unlockedNoteIDs = NotesManager.Instance.GetUnlockedNoteIDs();
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No NotesManager found—saving no unlocked notes.");
        }
    }

    private void SavePlayerStatsData(MasterSaveData data)
    {
        data.statsData = Player.Instance != null
            ? Player.Instance.GetComponent<PlayerStats>().GetStatsData()
            : new StatsData();
    }

    private void SaveLevelingData(MasterSaveData data)
    {
        data.levelingData = LevelingManager.Instance != null
            ? LevelingManager.Instance.GetLevelingData()
            : new LevelingData();
    }

    private void SaveAbilitiesData(MasterSaveData data)
    {
        data.abilitiesData = new AbilitiesData();
        if (AbilityManager.Instance != null)
        {
            data.abilitiesData.abilityNames = AbilityManager.Instance.GetCurrentAbilityNames();
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No AbilityManager found—saving no abilities.");
        }
    }

    private void SaveDungeonData(MasterSaveData data)
    {
        data.dungeonData = DungeonManager.Instance != null
            ? DungeonManager.Instance.CollectDungeonState()
            : null;
    }    

	public void Notes()
	{
		if (notesPanel.activeInHierarchy)
		{
			notesPanel.SetActive(false);
		}
		else
		{
			notesPanel.SetActive(true);
			NotesManager.Instance.OnNotesPanelOpened();
		}
	}

	public void Options()
	{
		if (optionsPanel.activeInHierarchy)
		{
			optionsPanel.SetActive(false);
			pausePanel.SetActive(true);
            UIManager.Instance.ChangeSelectedElement(ResumeButton);
        }
		else
		{
			optionsPanel.SetActive(true);
			pausePanel.SetActive(false);
			notesPanel.SetActive(false);
			controlsPanel.SetActive(false);
            UIManager.Instance.ChangeSelectedElement(OptionsButton);
        }
	}

	public void Hide()
	{
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
        notesPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

	public void MainMenu()
	{
		Debug.Log("Main Menu");
        SceneManager.LoadScene(0);
        GameManager.Instance.ResumeGameState();
    }

	public void Quit()
	{
		Debug.Log("Quit");
		Application.Quit();
	}
}
