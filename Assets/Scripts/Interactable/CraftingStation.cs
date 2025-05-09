using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : Interactable
{
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    public override void Interact()
    {
        base.Interact();

        if(UIManager.Instance != null)
        {
            UIManager.Instance.ToogleMenu(MenuType.CRAFTING);
            UIManager.Instance.DisplayRecipes(recipes);
        }
    }
}
