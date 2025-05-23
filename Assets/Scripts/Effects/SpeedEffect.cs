using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Effect", menuName = "Effects/Speed Effect")]
public class SpeedEffect : Effect
{
    public int amount = 1;

    public float effectTime = 3f;
    public override void UseEffect()
    {
        base.UseEffect();
        Player.Instance.GetComponent<PlayerStats>().ApplyEffectToMovementSpeed(amount, effectTime);
    }
}
