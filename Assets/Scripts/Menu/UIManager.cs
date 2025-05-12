using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuType
{
    NONE,
    PAUSE,
    INVENTORY,
    CRAFTING
}
public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup inventoryPanel;

    [SerializeField] private GameObject inventoryHolder;

    [SerializeField] private GameObject inventorySlot;

    [SerializeField] private GameObject tabHolder;

    [SerializeField] private GameObject statBlockPrefab;

    [SerializeField] private Transform statsHolder;

    [SerializeField] private EquipmentSlotHolder[] equipmentSlotHolders;

    [SerializeField] private EquipmentSlotHolder[] equipmentConsumableHolders;

    [SerializeField] private EquipmentSlotHolder[] equipmentSpellHolders;

    [SerializeField] private GameObject statsButton;

    [SerializeField] private CanvasGroup craftingPanel;

    [SerializeField] private GameObject resourcesHolder;

    [SerializeField] private Button craftingCloseButton;

    [SerializeField] private Transform recipesHolder;

    [SerializeField] private GameObject recipeBlockPrefab;

    [SerializeField] private CanvasGroup pausePanel;

    private Item lastUsedItem = null;

    private EventSystem eventSystem;

    private MenuType currentMenuType = MenuType.NONE;

    private InventoryTab currentInventoryTab = InventoryTab.ALL;

    public InventoryTab CurrentInventoryTab { get => currentInventoryTab; set => currentInventoryTab = value; }

    public Item LastUsedItem { get => lastUsedItem; set => lastUsedItem = value; }

    public static UIManager Instance { get; private set; }

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
        eventSystem = EventSystem.current;
    }

    private void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.alpha = 0f;
            inventoryPanel.blocksRaycasts = false;
            inventoryPanel.interactable = false;
        }

        if (craftingPanel != null)
        {
            craftingPanel.alpha = 0f;
            craftingPanel.blocksRaycasts = false;
            craftingPanel.interactable = false;
        }

        if (pausePanel != null)
        {
            pausePanel.alpha = 0f;
            pausePanel.blocksRaycasts = false;
            pausePanel.interactable = false;
        }

        if (Player.Instance != null)
        {
            BuildStatUI(Player.Instance.GetComponent<PlayerStats>());
        }

        if (craftingPanel != null)
        {
            craftingCloseButton.onClick.AddListener(delegate { ToogleMenu(MenuType.CRAFTING); });
        }
    }

    public void ChangeSelectedElement(GameObject toSelect)
    {
        eventSystem.SetSelectedGameObject(toSelect);
    }

    public void ToogleMenu(MenuType type)
    {
        if(currentMenuType == type)
        {
            CloseMenu(type);
            currentMenuType = MenuType.NONE;
        }
        else
        {
            if(currentMenuType == MenuType.NONE)
            {
                OpenMenu(type);
                currentMenuType = type;
            }
        }
    }

    private void OpenMenu(MenuType type)
    {
        switch(type)
        {
            case MenuType.PAUSE:
                if (NotesManager.Instance != null && pausePanel != null)
                {
                    pausePanel.alpha = 1f;
                    pausePanel.blocksRaycasts = true;
                    pausePanel.interactable = true;
                    ChangeSelectedElement(PauseManager.Instance.ResumeButton);
                }
                break;
            case MenuType.INVENTORY:
                if(InventoryManager.Instance != null && inventoryPanel != null)
                {
                    inventoryPanel.alpha = 1f;
                    inventoryPanel.blocksRaycasts = true;
                    inventoryPanel.interactable = true;
                    UpdateInventoryUI(InventoryTab.ALL);
                    currentInventoryTab = InventoryTab.ALL;
                    ChangeSelectedElement(statsButton);
                }
                break;
            case MenuType.CRAFTING:
                if (InventoryManager.Instance != null && craftingPanel != null)
                {
                    craftingPanel.alpha = 1f;
                    craftingPanel.blocksRaycasts = true;
                    craftingPanel.interactable = true;
                    UpdateCraftingUI();
                    ChangeSelectedElement(craftingCloseButton.gameObject);
                }
                break;
        }
        GameManager.Instance.PauseGameState();
    }

    private void CloseMenu(MenuType type)
    {
        switch (type)
        {
            case MenuType.PAUSE:
                if (NotesManager.Instance != null && pausePanel != null)
                {
                    pausePanel.alpha = 0f;
                    pausePanel.blocksRaycasts = false;
                    pausePanel.interactable = false;
                    PauseManager.Instance.Hide();
                }
                break;
            case MenuType.INVENTORY:
                if (InventoryManager.Instance != null && inventoryPanel != null)
                {
                    inventoryPanel.alpha = 0f;
                    inventoryPanel.blocksRaycasts = false;
                    inventoryPanel.interactable = false;
                }
                break;
            case MenuType.CRAFTING:
                if (InventoryManager.Instance != null && craftingPanel != null)
                {
                    craftingPanel.alpha = 0f;
                    craftingPanel.blocksRaycasts = false;
                    craftingPanel.interactable = false;
                }
                break;
        }
        GameManager.Instance.ResumeGameState();
    }

    public void ChangeInventoryTab(InventoryTab tab)
    {
        UpdateInventoryUI(tab);
        currentInventoryTab = tab;
        ChangeSelectedElement(tabHolder.transform.GetChild((int)tab).gameObject);
    }

    public void UpdateInventoryUI(InventoryTab tab)
    {
        if(inventoryHolder == null)
        {
            return;
        }

        foreach (Transform child in inventoryHolder.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject slotToReselect = null;

        foreach (Item item in InventoryManager.Instance.Items)
        {
            switch(tab)
            {
                case InventoryTab.ALL:
                    break;
                case InventoryTab.EQUIPMENT:
                    if(item.Type != ItemType.EQUIPMENT)
                    {
                        continue;
                    }
                    break;
                case InventoryTab.CONSUMABLES:
                    if (item.Type != ItemType.CONSUMABLE)
                    {
                        continue;
                    }
                    break;
                case InventoryTab.SPELLS:
                    if (item.Type != ItemType.SPELL)
                    {
                        continue;
                    }
                    break;
                case InventoryTab.RESOURCES:
                    if (item.Type != ItemType.RESOURCE)
                    {
                        continue;
                    }
                    break;
            }
            GameObject go = Instantiate(inventorySlot, inventoryHolder.transform);
            InventorySlot slot = go.GetComponent<InventorySlot>();
            slot.AddItem(item);
            slot.UpdateEquippedUI();

            if(lastUsedItem != null)
            {
                if (item.Name == lastUsedItem.Name)
                {
                    slotToReselect = go;
                }
            }
        }

        if (slotToReselect != null)
        {
            ChangeSelectedElement(slotToReselect);
        }
        else
        {
            ChangeSelectedElement(tabHolder.transform.GetChild(0).gameObject);
        }

        lastUsedItem = null;
    }

    public void UpdateCraftingUI()
    {
        if (resourcesHolder == null)
        {
            return;
        }

        foreach (Transform child in resourcesHolder.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in InventoryManager.Instance.Items)
        {
            if (item.Type != ItemType.RESOURCE)
            {
                continue;
            }
            GameObject go = Instantiate(inventorySlot, resourcesHolder.transform);
            InventorySlot slot = go.GetComponent<InventorySlot>();
            slot.AddItem(item);
        }

        ChangeSelectedElement(craftingCloseButton.gameObject);

        lastUsedItem = null;
    }

    public void DisplayRecipes(List<Recipe> recipes)
    {
        foreach (Transform child in recipesHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (Recipe recipe in recipes)
        {
            GameObject go = Instantiate(recipeBlockPrefab, recipesHolder);
            go.GetComponent<RecipeBlock>().SetUpRecipe(recipe);
        }
    }

    public void UpdateEquipmentSlotHolder(Item item, bool equipped)
    {
        if(item.Type == ItemType.EQUIPMENT)
        {
            Equipment eq = (Equipment)item;
            if(equipped)
            {
                equipmentSlotHolders[(int)eq.EqSlot].AddItem(item);
            }
            else
            {
                equipmentSlotHolders[(int)eq.EqSlot].RemoveItem();
            }
        }
        else if(item.Type == ItemType.CONSUMABLE)
        {
            Consumable co = (Consumable)item;

            if(equipped)
            {
                if (co.slot == 1)
                {
                    equipmentConsumableHolders[0].AddItem(item);
                }
                else if(co.slot == 2)
                {
                    equipmentConsumableHolders[1].AddItem(item);
                }
            }
            else
            {
                if (co.slot == 1)
                {
                    equipmentConsumableHolders[0].RemoveItem();
                }
                else if (co.slot == 2)
                {
                    equipmentConsumableHolders[1].RemoveItem();
                }
                co.slot = 0;
            }
        }
        else if(item.Type == ItemType.SPELL)
        {
            Spell sp = (Spell)item;

            if (equipped)
            {
                if (sp.slot == 1)
                {
                    equipmentSpellHolders[0].AddItem(item);
                }
                else if (sp.slot == 2)
                {
                    equipmentSpellHolders[1].AddItem(item);
                }
            }
            else
            {
                if (sp.slot == 1)
                {
                    equipmentSpellHolders[0].RemoveItem();
                }
                else if (sp.slot == 2)
                {
                    equipmentSpellHolders[1].RemoveItem();
                }
                sp.slot = 0;
            }
        }
    }

    private void BuildStatUI(PlayerStats playerStats)
    {
        foreach (Transform t in statsHolder)
        {
            Destroy(t.gameObject);
        }
        
        CreateStatBlock(
            "Health",
            () => $"{playerStats.CurrentHealth}/{playerStats.MaxHealth.GetValue()}",
            () => playerStats.UpgradeMaxHealth(50),
            playerStats.HealthChanged
        );

        // Dodge Charges
        CreateStatBlock(
            "Dodge Charges",
            () => $"{playerStats.CurrentDodgeCharges}/{playerStats.MaxDodgeCharges.GetValue()}",
            () => playerStats.UpgradeMaxDodgeCharges(1),
            playerStats.DodgeChargesChanged
        );

        // Armor
        CreateStatBlock(
            "Armor",
            () => playerStats.Armor.GetValue().ToString(),
            () => playerStats.Armor.Upgrade(10),
            playerStats.Armor.ValueChanged
        );

        // Damage
        CreateStatBlock(
            "Damage",
            () => playerStats.Damage.GetValue().ToString(),
            () => playerStats.Damage.Upgrade(10),
            playerStats.Damage.ValueChanged
        );

        // Movement Speed
        CreateStatBlock(
            "Move Speed",
            () => playerStats.MovementSpeed.GetValue().ToString(),
            () => playerStats.MovementSpeed.Upgrade(1),
            playerStats.MovementSpeed.ValueChanged
        );
    }

    private void CreateStatBlock(string label,
                                 Func<string> readout,
                                 Action onUpgrade,
                                 UnityEvent changeEvent)
    {
        var go = Instantiate(statBlockPrefab, statsHolder);
        var sb = go.GetComponent<StatBlock>();
        sb.Initialize(label, readout, onUpgrade);
        changeEvent.AddListener(sb.Refresh);
    }

    private void CreateStatBlock(string label,
                                 Func<string> readout,
                                 Action onUpgrade,
                                 UnityEvent<int, int> changeEvent)
    {
        var go = Instantiate(statBlockPrefab, statsHolder);
        var sb = go.GetComponent<StatBlock>();
        sb.Initialize(label, readout, onUpgrade);
        changeEvent.AddListener((cur, max) => sb.Refresh());
    }
}

public enum InventoryTab
{
    ALL,
    EQUIPMENT,
    CONSUMABLES,
    SPELLS,
    RESOURCES
}