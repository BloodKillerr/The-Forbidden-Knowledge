using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    public Item Result;

    public List<RecipeComponent> Components = new List<RecipeComponent>();

    public void TryCrafting()
    {
        if (!InventoryManager.Instance.ContainsItem(Result, 1))
        {
            foreach (RecipeComponent component in Components)
            {
                if(!InventoryManager.Instance.ContainsItem(component.Item, component.Amount))
                {
                    return;
                }
            }

            foreach (RecipeComponent component in Components)
            {
                InventoryManager.Instance.Remove(component.Item, component.Amount);
            }

            InventoryManager.Instance.Add(Instantiate(Result));
        }
    }

    public bool CanCraft()
    {
        foreach (RecipeComponent component in Components)
        {
            if (!InventoryManager.Instance.ContainsItem(component.Item, component.Amount))
            {
                return false;
            }
        }
        return true;
    }
}
