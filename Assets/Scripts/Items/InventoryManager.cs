using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();

    private bool isShown = false;

    public bool IsShown { get => isShown; set => isShown = value; }

    public List<Item> Items { get => items; set => items = value; }

    public static InventoryManager Instance { get; private set; }

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

    public void Add(Item item)
    {
        Item copy = Instantiate(item);
        bool itemInInventory = false;
        foreach (Item inventoryItem in items)
        {
            if(inventoryItem.Name == copy.Name)
            {
                inventoryItem.Amount += copy.Amount;
                itemInInventory = true;
                break;
            }
        }

        if (!itemInInventory)
        {
            items.Add(copy);
        }
    }

    public bool Remove(Item item, int amount)
    {
        foreach (Item inventoryItem in items)
        {
            if(inventoryItem.Name == item.Name)
            {
                if(inventoryItem.Amount < amount)
                {
                    return false;
                }

                inventoryItem.Amount -= amount;

                if(inventoryItem.Amount <= 0)
                {
                    items.Remove(inventoryItem);
                }
                return true;
            }
        }
        return false;
    }
}
