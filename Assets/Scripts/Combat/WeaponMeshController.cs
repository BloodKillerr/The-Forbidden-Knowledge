using UnityEngine;

public class WeaponMeshController : MonoBehaviour
{
    [SerializeField] private Transform weaponHolster;
    [SerializeField] private Transform hand;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;

    private GameObject currentHolsteredMesh;
    private GameObject currentHandMesh;

    private DamageCollider primaryWeaponCollider;

    public Weapon PrimaryWeapon { get => primaryWeapon; set => primaryWeapon = value; }
    public Weapon SecondaryWeapon { get => secondaryWeapon; set => secondaryWeapon = value; }

    public void SetPrimaryWeapon(Weapon weapon)
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

    public void SetSecondaryWeapon(Weapon weapon)
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

    public void AttackWithPrimaryWeapon()
    {
        if (currentHolsteredMesh != null)
        {
            Destroy(currentHolsteredMesh);
        }

        if (currentHandMesh != null)
        {
            Destroy(currentHandMesh);
        }

        currentHandMesh = Instantiate(primaryWeapon.mesh, hand);
        currentHandMesh.transform.localPosition = primaryWeapon.HandPositionOffset;
        currentHandMesh.transform.localEulerAngles = primaryWeapon.HandRotationOffset;
        primaryWeaponCollider = currentHandMesh.GetComponentInChildren<DamageCollider>();
    }

    public void EnablePrimaryDamageCollider()
    {
        primaryWeaponCollider.EnableDamageCollider();
    }

    public void DisablePrimaryDamageCollider()
    {
        primaryWeaponCollider.DisableDamageCollider();
    }

    public void AttackWithSecondaryWeapon()
    {

    }

    public void HolsterPrimaryWeapon()
    {
        if (currentHandMesh != null)
        {
            Destroy(currentHandMesh);
        }

        if(primaryWeapon != null)
        {
            currentHolsteredMesh = Instantiate(primaryWeapon.mesh, weaponHolster);
            currentHolsteredMesh.transform.localPosition = primaryWeapon.HolsterPositionOffset;
            currentHolsteredMesh.transform.localEulerAngles = primaryWeapon.HolsterRotationOffset;
        }
        primaryWeaponCollider = null;
    }

    public void HideSecondaryWeapon()
    {

    }
}
