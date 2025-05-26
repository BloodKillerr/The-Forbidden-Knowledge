using System;
using System.Collections;
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

public enum InventoryTab
{
    ALL,
    EQUIPMENT,
    CONSUMABLES,
    SPELLS,
    RESOURCES
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

    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthFrame;
    [SerializeField] private Color normalHealthColor = Color.white;
    [SerializeField] private Color invincibleHealthColor = Color.yellow;

    [Header("Dodge Charges")]
    [SerializeField] private Transform dodgeContainer;
    [SerializeField] private GameObject dodgeChargePrefab;

    [Header("XP & Stat Points")]
    [SerializeField] private Image xpFillImage;
    [SerializeField] private TMP_Text statPointsText;

    [Header("Consumable Cooldowns")]
    [SerializeField] private Image[] consumableImages = new Image[2];
    [SerializeField] private Slider[] consumableCooldownSliders = new Slider[2];

    [Header("Spell Cooldowns")]
    [SerializeField] private Image[] spellImages = new Image[2];
    [SerializeField] private Slider[] spellCooldownSliders = new Slider[2];

    [Header("Pickup Messages")]
    [SerializeField] private RectTransform messageBoxContainer;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private float messageDisplayTime = 2f;
    [SerializeField] private int maxConcurrentMessages = 3;
    private Queue<GameObject> pendingMessages = new Queue<GameObject>();
    private List<GameObject> activeMessages = new List<GameObject>();
    private bool processingQueue = false;

    private Item lastUsedItem = null;

    private EventSystem eventSystem;

    private MenuType currentMenuType = MenuType.NONE;

    private InventoryTab currentInventoryTab = InventoryTab.ALL;

    public InventoryTab CurrentInventoryTab { get => currentInventoryTab; set => currentInventoryTab = value; }

    public Item LastUsedItem { get => lastUsedItem; set => lastUsedItem = value; }

    [SerializeField] private GameObject TooltipGO;

    public ToolTipUI Tooltip;

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
        Tooltip = TooltipGO.GetComponent<ToolTipUI>();
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

        Tooltip.Hide();

        if (Player.Instance == null)
        {
            return;
        }
        PlayerStats playerStats = Player.Instance.GetComponent<PlayerStats>();

        playerStats.HealthChanged.AddListener(OnHealthChanged);
        playerStats.ApplyInvincibilityEvent.AddListener(OnInvincibilityStarted);
        OnHealthChanged(playerStats.CurrentHealth, playerStats.MaxHealth.GetValue());

        playerStats.DodgeChargesChanged.AddListener(OnDodgeChargesChanged);
        OnDodgeChargesChanged(playerStats.CurrentDodgeCharges, playerStats.MaxDodgeCharges.GetValue());

        LevelingManager levelingManager = LevelingManager.Instance;
        levelingManager.XPChanged.AddListener(OnXPChanged);
        levelingManager.StatPointsChanged.AddListener(OnStatPointsChanged);
        OnXPChanged(levelingManager.CurrentXP, levelingManager.RequiredXP);
        OnStatPointsChanged(levelingManager.StatPoints);

        InventoryManager.Instance.OnItemAdded.AddListener(OnItemAdded);

        for (int i = 0; i < 2; i++)
        {
            consumableImages[i].gameObject.SetActive(false);
            consumableCooldownSliders[i].gameObject.SetActive(false);
            spellImages[i].gameObject.SetActive(false);
            spellCooldownSliders[i].gameObject.SetActive(false);
        }

        ConsumableManager consumableManager = ConsumableManager.Instance;
        consumableManager.OnConsumableEquipped.AddListener(OnConsumableEquipped);
        consumableManager.OnConsumableUnequipped.AddListener(OnConsumableUnequipped);
        consumableManager.OnConsumableUsed.AddListener(OnConsumableUsed);

        SpellManager spellManager = SpellManager.Instance;
        spellManager.OnSpellEquipped.AddListener(OnSpellEquipped);
        spellManager.OnSpellUnequipped.AddListener(OnSpellUnequipped);
        spellManager.OnSpellUsed.AddListener(OnSpellUsed);
    }

    #region Health & Invincibility

    private void OnHealthChanged(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
        healthText.text = $"{current} / {max}";
    }

    private void OnInvincibilityStarted(float duration)
    {
        healthFrame.color = invincibleHealthColor;
        StartCoroutine(ResetHealthFrameAfter(duration));
    }

    private IEnumerator ResetHealthFrameAfter(float t)
    {
        yield return new WaitForSeconds(t);
        healthFrame.color = normalHealthColor;
    }

    public void ShowInvincibilityFrame()
    {
        healthFrame.color = invincibleHealthColor;
    }

    public void HideInvincibilityFrame()
    {
        healthFrame.color = normalHealthColor;
    }

    #endregion

    #region Dodge Charges

    private void OnDodgeChargesChanged(int current, int max)
    {
        foreach (Transform t in dodgeContainer)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < max; i++)
        {
            GameObject go = Instantiate(dodgeChargePrefab, dodgeContainer);
            Image img = go.GetComponent<Image>();
            img.color = (i < current) ? Color.white : Color.gray;
        }
    }

    #endregion

    #region XP & Stat Points

    private void OnXPChanged(int currentXP, int requiredXP)
    {
        xpFillImage.fillAmount = requiredXP > 0 ? (float)currentXP / requiredXP : 0f;
    }

    private void OnStatPointsChanged(int points)
    {
        statPointsText.text = points.ToString();
    }

    #endregion

    #region Consumable & Spell Cooldowns

    private void OnConsumableEquipped(int slot, Consumable item)
    {
        int idx = slot - 1;
        consumableImages[idx].sprite = item.Icon;
        consumableImages[idx].gameObject.SetActive(true);

        if (consumableCooldownSliders[idx].gameObject.activeSelf)
        {
            return;
        }

        consumableCooldownSliders[idx].gameObject.SetActive(false);
    }

    private void OnConsumableUnequipped(int slot)
    {
        int idx = slot - 1;
        consumableImages[idx].gameObject.SetActive(false);

        if (consumableCooldownSliders[idx].gameObject.activeSelf)
        {
            return;
        }

        consumableCooldownSliders[idx].gameObject.SetActive(false);
    }

    private void OnSpellEquipped(int slot, Spell item)
    {
        int idx = slot - 1;
        spellImages[idx].sprite = item.Icon;
        spellImages[idx].gameObject.SetActive(true);

        if (spellCooldownSliders[idx].gameObject.activeSelf)
        {
            return;
        }

        spellCooldownSliders[idx].gameObject.SetActive(false);
    }

    private void OnSpellUnequipped(int slot)
    {
        int idx = slot - 1;
        spellImages[idx].gameObject.SetActive(false);

        if (spellCooldownSliders[idx].gameObject.activeSelf)
        {
            return;
        }

        spellCooldownSliders[idx].gameObject.SetActive(false);
    }

    private void OnConsumableUsed(int slot, Consumable item)
    {
        int idx = slot - 1;
        StartCoroutine(RunCooldown(item.Cooldown, idx, consumableImages, consumableCooldownSliders, item.Icon));
    }

    private void OnSpellUsed(int slot, Spell item)
    {
        int idx = slot - 1;
        StartCoroutine(RunCooldown(item.Cooldown, idx, spellImages, spellCooldownSliders, item.Icon));
    }

    private IEnumerator RunCooldown(float cooldown, int idx, Image[] images, Slider[] sliders, Sprite icon)
    {
        images[idx].sprite = icon;
        images[idx].gameObject.SetActive(true);

        sliders[idx].maxValue = cooldown;
        sliders[idx].value = cooldown;
        sliders[idx].gameObject.SetActive(true);

        float remaining = cooldown;
        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            sliders[idx].value = Mathf.Max(0f, remaining);
            yield return null;
        }

        sliders[idx].gameObject.SetActive(false);
    }

    public void ResetAllCooldownUI()
    {
        for (int i = 0; i < consumableCooldownSliders.Length; i++)
        {
            consumableCooldownSliders[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < spellCooldownSliders.Length; i++)
        {
            spellCooldownSliders[i].gameObject.SetActive(false);
        }
    }

    public void RefreshQuickbarUI()
    {
        ResetAllCooldownUI();

        StopAllCoroutines();
    }

    #endregion

    #region Pickup Message Queue

    private void OnItemAdded(Item item)
    {
        GameObject go = Instantiate(messagePrefab, messageBoxContainer);
        go.SetActive(false);
        TMP_Text txt = go.GetComponentInChildren<TMP_Text>();
        txt.text = $"Picked up {item.Name} x{item.Amount}";

        pendingMessages.Enqueue(go);

        if (!processingQueue)
        {
            StartCoroutine(ProcessMessageQueue());
        }
    }

    private IEnumerator ProcessMessageQueue()
    {
        processingQueue = true;

        while (pendingMessages.Count > 0)
        {
            GameObject msg = pendingMessages.Dequeue();

            if (activeMessages.Count >= maxConcurrentMessages)
            {
                GameObject oldest = activeMessages[0];
                activeMessages.RemoveAt(0);
                Destroy(oldest);
            }

            msg.SetActive(true);
            activeMessages.Add(msg);

            yield return new WaitForSeconds(messageDisplayTime);
        }

        while (activeMessages.Count > 0)
        {
            GameObject oldest = activeMessages[0];
            activeMessages.RemoveAt(0);
            Destroy(oldest);
            yield return new WaitForSeconds(messageDisplayTime);
        }

        processingQueue = false;
    }

    public void ClearMessages()
    {
        pendingMessages.Clear();
        activeMessages.Clear();
        processingQueue = false;

        foreach(Transform child in messageBoxContainer)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

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
                    Tooltip.Hide();
                }
                break;
            case MenuType.CRAFTING:
                if (InventoryManager.Instance != null && craftingPanel != null)
                {
                    craftingPanel.alpha = 0f;
                    craftingPanel.blocksRaycasts = false;
                    craftingPanel.interactable = false;
                    Tooltip.Hide();
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
            () => playerStats.UpgradeMaxHealth(60),
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
            () => playerStats.Armor.Upgrade(8),
            playerStats.Armor.ValueChanged
        );

        // Damage
        CreateStatBlock(
            "Damage",
            () => playerStats.Damage.GetValue().ToString(),
            () => playerStats.Damage.Upgrade(12),
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