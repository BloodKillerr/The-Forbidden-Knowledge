using UnityEngine;

[CreateAssetMenu(fileName = "New Freeze Effect", menuName = "Effects/Freeze Effect")]
public class FreezeEffect : Effect
{
    public float radius = 4f;
    public float duration = 3f;
    public LayerMask enemyLayer;

    public override void UseEffect()
    {
        base.UseEffect();
        Collider[] hits = Physics.OverlapSphere(Player.Instance.gameObject.transform.position, radius, enemyLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Enemy e = hits[i].GetComponent<Enemy>();
            if (e != null)
            {
                e.Freeze(duration);
            }
        }
    }
}
