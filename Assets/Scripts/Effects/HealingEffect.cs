using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Effect", menuName = "Effects/Healing Effect")]
public class HealingEffect : Effect
{
    [Range(0.0f, 1.0f)]
    public float HealAmount = .5f;
    public override void UseEffect()
    {
        base.UseEffect();
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.Heal((int)(stats.MaxHealth.GetValue()*HealAmount));
    }
}
