using UnityEngine;

[CreateAssetMenu(fileName = "New Fireball Effect", menuName = "Effects/Fireball Effect")]
public class FireBallEffet : Effect
{
    public GameObject FireballPrefab;
    public override void UseEffect()
    {
        base.UseEffect();
        Instantiate(FireballPrefab, new Vector3(Player.Instance.gameObject.transform.position.x,
            Player.Instance.gameObject.transform.position.y+1f, 
            Player.Instance.gameObject.transform.position.z), Player.Instance.gameObject.transform.rotation);
    }
}
