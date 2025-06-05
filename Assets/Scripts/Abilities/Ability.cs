using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("UI/Metadata")]
    public string AbilityName;
    public Sprite Icon;
    public string Description;

    public enum AbilityType { Passive, Active }
    public AbilityType type;

    public virtual void OnEquip() { }
    public virtual void OnUnequip() { }
    public virtual void OnPlayerAttack() { }
    public virtual void OnEnemyKilled() { }
}
