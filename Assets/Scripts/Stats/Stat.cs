using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    private List<int> modifiers = new List<int>();

    [SerializeField] private int valueUpgradeCap;

    public UnityEvent ValueChanged = new UnityEvent();

    public int GetValue()
    {
        int finalValue = baseValue;

        modifiers.ForEach(x => finalValue += x);

        return finalValue;
    }

    public void AddModifier(int modifier)
    {
        if(modifier != 0)
        {
            modifiers.Add(modifier);
            ValueChanged?.Invoke();
        }
    }

    public void RemoveModifier(int modifier)
    {
        if (modifier != 0)
        {
            modifiers.Remove(modifier);
            ValueChanged?.Invoke();
        }
    }

    public bool Upgrade(int upgrade)
    {
        if(baseValue < valueUpgradeCap && LevelingManager.Instance.UseStatPoint())
        {
            baseValue = Mathf.Min(baseValue + upgrade, valueUpgradeCap);
            ValueChanged?.Invoke();
            return true;
        }
        return false;
    }
}
