using UnityEngine;

[CreateAssetMenu(fileName = "New Invincibility Effect", menuName = "Effects/Invincibility Effect")]
public class InvincibilityEffect : Effect
{
    public float duration = 2f;

    public override void UseEffect()
    {
        base.UseEffect();
        Player.Instance.GetComponent<PlayerStats>().ApplyInvincibility(duration);
    }
}
