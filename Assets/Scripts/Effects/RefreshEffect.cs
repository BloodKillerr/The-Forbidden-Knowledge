using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Effect", menuName = "Effects/Refresh Effect")]
public class RefreshEffect : Effect
{
    public float amount = 10f;

    public override void UseEffect()
    {
        base.UseEffect();
        SpellManager.Instance.ShortenCooldown(amount);
        ConsumableManager.Instance.ShortenCooldown(amount);
    }
}
