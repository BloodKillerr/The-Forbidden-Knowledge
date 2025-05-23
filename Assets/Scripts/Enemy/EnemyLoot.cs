using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [SerializeField] private List<Loot> lootTable = new List<Loot>();

    [SerializeField] private int xpReward = 5;

    public void GetLoot()
    {
        foreach (Loot entry in lootTable)
        {
            if (Random.value <= entry.DropChance)
            {
                int count = Random.Range(entry.MinAmount, entry.MaxAmount + 1);

                if (entry.Item.Type != ItemType.RESOURCE && count > 1)
                {
                    continue;
                }

                Item copy = Instantiate(entry.Item);
                copy.Amount = count;
                InventoryManager.Instance.Add(copy);
            }
        }

        LevelingManager.Instance.GainXP(xpReward);
    }
}
