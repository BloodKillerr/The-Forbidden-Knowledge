using UnityEngine;

[CreateAssetMenu(
    fileName = "InstantiateAbility",
    menuName = "Abilities/Active/InstantiateAbility"
)]
public class InstantiateAbility : ActiveAbility
{
    public GameObject Prefab;

    public Vector3 Offset = new Vector3(0f, 1f, 0f);
    public override void OnPlayerAttack()
    {
        if (Prefab == null)
        {
            return;
        }

        GameObject go = Instantiate(
            Prefab,
            Player.Instance.gameObject.transform.position + Offset,
            Player.Instance.gameObject.transform.rotation
        );
    }
}
