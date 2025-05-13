using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : Interactable
{
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    public override void Interact()
    {
        if (Player.Instance.GetComponent<PlayerMovement>().IsDodging || Player.Instance.GetComponent<PlayerAttack>().IsAttacking)
        {
            return;
        }
        base.Interact();

        if(UIManager.Instance != null)
        {
            UIManager.Instance.ToogleMenu(MenuType.CRAFTING);
            UIManager.Instance.DisplayRecipes(recipes);
        }
    }
}
