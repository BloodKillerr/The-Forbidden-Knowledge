using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBlock : MonoBehaviour
{
    public Recipe Recipe;

    public Image ResultSlotItemIcon;

    public Transform ComponentsBlock;

    private void Start()
    {
        if(InventoryManager.Instance.ContainsItem(Recipe.Result, 1))
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void SetUpRecipe(Recipe recipe)
    {
        Recipe = recipe;
        ResultSlotItemIcon.sprite = recipe.Result.Icon;

        int i = 0;
        for (i = 0; i < recipe.Components.Count; i++)
        {
            Transform root = ComponentsBlock.GetChild(i);

            foreach (Transform child in root)
            {
                Image imange = child.GetComponent<Image>();
                if (imange != null)
                {
                    imange.sprite = recipe.Components[i].Item.Icon;
                }

                TMP_Text text = child.GetComponent<TMP_Text>();
                if (text != null)
                {
                    text.text = String.Format("x{0}", recipe.Components[i].Amount);
                }
            }
        }
        for(int j = i; j<ComponentsBlock.childCount; j++)
        {
            Destroy(ComponentsBlock.GetChild(j).gameObject);
        }
    }

    public void Craft()
    {
        if(!InventoryManager.Instance.ContainsItem(Recipe.Result, 1))
        {
            Recipe.TryCrafting();
            if(InventoryManager.Instance.ContainsItem(Recipe.Result, 1))
            {
                GetComponent<Button>().interactable = false;
                UIManager.Instance.UpdateCraftingUI();
            }
        }
    }
}