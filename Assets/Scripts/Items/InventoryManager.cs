using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();

    [SerializeField] private Item startingItem;

    public List<Item> Items { get => items; set => items = value; }

    public UnityEvent<Item> OnItemAdded = new UnityEvent<Item>();

    public static InventoryManager Instance { get; private set; }

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
        Add(startingItem);
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
        OnItemAdded.Invoke(copy);
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

    public bool ContainsItem(Item item, int amount)
    {
        foreach(Item inventoryItem in items)
        {
            if(inventoryItem.Name == item.Name)
            {
                if(inventoryItem.Amount >= amount)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
