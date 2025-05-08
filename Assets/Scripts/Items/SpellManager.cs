using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private Spell spell1 = null;
    private Spell spell2 = null;

    public static SpellManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void EquipSpell(Spell spell)
    {
        if (spell1 == null)
        {
            spell1 = spell;
            spell.slot = 1;
        }
        else if (spell2 == null)
        {
            spell2 = spell;
            spell.slot = 2;
        }
        else
        {
            spell1.Equipped = false;
            spell1 = spell;
            spell.slot = 1;
        }

        spell.Equipped = true;
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UnEquipSpell(Spell spell)
    {
        if (spell1 != null && spell1.Name == spell.Name)
        {
            spell1.Equipped = false;
            spell1 = null;
        }
        else if (spell2 != null && spell2.Name == spell.Name)
        {
            spell2.Equipped = false;
            spell2 = null;
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UnEquipAll()
    {
        if (spell1 != null)
        {
            spell1.Equipped = false;
            spell1 = null;
        }
        if (spell2 != null)
        {
            spell2.Equipped = false;
            spell2 = null;
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }
}
