using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private List<Loot> lootTable = new List<Loot>();

    public List<Loot> LootTable { get => lootTable; set => lootTable = value; }

    public override void Interact()
    {
        if (Player.Instance.GetComponent<PlayerMovement>().IsDodging || Player.Instance.GetComponent<PlayerAttack>().IsAttacking)
        {
            return;
        }
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

    public void AddItem(Item item, int minAmount, int maxAmount, float dropChance)
    {
        lootTable.Add(new Loot(Instantiate(item), minAmount, maxAmount, dropChance));
    }
}
