using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup inventoryPanel;

    [SerializeField] private GameObject inventoryHolder;

    [SerializeField] private GameObject inventorySlot;

    private EventSystem eventSystem;

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

    public void ShowHideInventory()
    {
        if(InventoryManager.Instance == null)
        {
            return;
        }

        if(InventoryManager.Instance.IsShown)
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.alpha = 0f;
                inventoryPanel.blocksRaycasts = false;
                inventoryPanel.interactable = false;
                GameManager.Instance.ResumeGameState();
            }
        }
        else
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.alpha = 1f;
                inventoryPanel.blocksRaycasts = true;
                inventoryPanel.interactable = true;
                GameManager.Instance.PauseGameState();
                UpdateInventoryUI(InventoryTab.ALL);
            }
        }
        InventoryManager.Instance.IsShown = !InventoryManager.Instance.IsShown;
    }

    public void ChangeInventoryTab(InventoryTab tab)
    {
        UpdateInventoryUI(tab);
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
            slot.AddItem(Instantiate(item));
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