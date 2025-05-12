using UnityEngine;

public class WeaponMeshController : MonoBehaviour
{
    [SerializeField] private Transform weaponHolster;
    [SerializeField] private Transform hand;

    private Equipment primaryWeapon;
    private Equipment secondaryWeapon;

    private GameObject currentHolsteredMesh;

    public void SetPrimaryWeapon(Equipment weapon)
    {
        if (currentHolsteredMesh != null)
        {
            Destroy(currentHolsteredMesh);
        }

        if (weapon == null)
        {
            primaryWeapon = null;
            return;
        }

        primaryWeapon = weapon;

        if (weapon.mesh != null)
        {
            currentHolsteredMesh = Instantiate(weapon.mesh, weaponHolster);
            currentHolsteredMesh.transform.localPosition = weapon.HolsterPositionOffset;
            currentHolsteredMesh.transform.localEulerAngles = weapon.HolsterRotationOffset;
        }
    }

    public void SetSecondaryWeapon(Equipment weapon)
    {
        if (weapon == null)
        {
            secondaryWeapon = null;
        }
        else
        {
            secondaryWeapon = weapon;
        }
    }
}
