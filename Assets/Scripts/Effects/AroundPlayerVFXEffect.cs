using UnityEngine;

[CreateAssetMenu(fileName = "New AroundPlayerVFX Effect", menuName = "Effects/AroundPlayerVFX Effect")]
public class AroundPlayerVFXEffect : Effect
{
    public GameObject effect;

    public float delay = 3f;

    public override void UseEffect()
    {
        base.UseEffect();
        GameObject go = Instantiate(effect, new Vector3(Player.Instance.gameObject.transform.position.x,
            Player.Instance.gameObject.transform.position.y + 1f,
            Player.Instance.gameObject.transform.position.z), Player.Instance.gameObject.transform.rotation, Player.Instance.transform);

        Destroy(go, delay);
    }
}
