using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Effect", menuName = "Effects/Healing Effect")]
public class HealingEffect : Effect
{
    public int HealAmount = 50;
    public override void UseEffect()
    {
        base.UseEffect();
        Player.Instance.GetComponent<PlayerStats>().Heal(HealAmount);
    }
}
