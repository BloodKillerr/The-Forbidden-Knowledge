using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    [SerializeField] private EquipmentSlotHolder[] equipmentSlotHolders;

    [SerializeField] private EquipmentSlotHolder[] equipmentConsumableHolders;

    [SerializeField] private EquipmentSlotHolder[] equipmentSpellHolders;

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
            Destroy(this);
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
                break;
            case MenuType.INVENTORY:
                if(InventoryManager.Instance != null && inventoryPanel != null)
                {
                    inventoryPanel.alpha = 1f;
                    inventoryPanel.blocksRaycasts = true;
                    inventoryPanel.interactable = true;
                    UpdateInventoryUI(InventoryTab.ALL);
                    currentInventoryTab = InventoryTab.ALL;
                    ChangeSelectedElement(tabHolder.transform.GetChild(0).gameObject);
                }
                break;
            case MenuType.CRAFTING:
                break;
        }
        GameManager.Instance.PauseGameState();
    }

    private void CloseMenu(MenuType type)
    {
        switch (type)
        {
            case MenuType.PAUSE:
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

        }
        else if(item.Type == ItemType.SPELL)
        {

        }
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