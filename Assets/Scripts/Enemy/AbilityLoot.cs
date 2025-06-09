using System;
using UnityEngine;

[Serializable]
public class AbilityLoot
{
    public Ability Ability;

    [Range(0f, 1f)]
    public float DropChance = 1f;

    public AbilityLoot(Ability ability, float dropChance)
    {
        Ability = ability;
        DropChance = dropChance;
    }
}
