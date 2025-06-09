using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Damage Effect")]
public class DamageEffect : Effect
{
    [Range(0.0f, 1.0f)]
    public float amount = .2f;

    public float effectTime = 3f;

    public override void UseEffect()
    {
        base.UseEffect();
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.ApplyEffectToDamage((int)(stats.Damage.GetValue()*amount), effectTime);
    }
}
