using System;
using UnityEngine;

[Serializable]
public class Loot
{
    public Item Item;
    
    public int MinAmount = 1;

    public int MaxAmount = 3;

    [Range(0f, 1f)]
    public float DropChance = 1f;
}
