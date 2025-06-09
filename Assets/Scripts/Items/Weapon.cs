using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment
{
    public GameObject mesh;

    public Vector3 HolsterPositionOffset;
    public Vector3 HolsterRotationOffset;

    public Vector3 HandPositionOffset;
    public Vector3 HandRotationOffset;

    [Header("Attack Animations")]
    public string[] AttackAnimations;
    public float ComboResetTime = 1f;

    public SoundType SoundType;
}
