using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private List<Loot> lootTable = new List<Loot>();
    public override void Interact()
    {
        base.Interact();
        foreach (Loot entry in lootTable)
        {
            if (Random.value <= entry.DropChance)
            {
                int count = Random.Range(entry.MinAmount, entry.MaxAmount + 1);

                if(entry.Item.Type != ItemType.RESOURCE && count > 1)
                {
                    continue;
                }

                Item copy = Instantiate(entry.Item);
                copy.Amount = count;
                InventoryManager.Instance.Add(copy);
            }
        }
        Destroy(gameObject);
    }
}
