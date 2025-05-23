using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Damage Effect")]
public class DamageEffect : Effect
{
    public int amount = 10;

    public float effectTime = 3f;

    public override void UseEffect()
    {
        base.UseEffect();
        Player.Instance.GetComponent<PlayerStats>().ApplyEffectToDamage(amount, effectTime);
    }
}
