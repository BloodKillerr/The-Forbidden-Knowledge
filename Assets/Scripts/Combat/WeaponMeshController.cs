using UnityEngine;

public class WeaponMeshController : MonoBehaviour
{
    [SerializeField] private Transform weaponHolster;
    [SerializeField] private Transform hand;

    private Weapon primaryWeapon;

    private GameObject currentHolsteredMesh;
    private GameObject currentHandMesh;

    private DamageCollider primaryWeaponCollider;

    public Weapon PrimaryWeapon { get => primaryWeapon; set => primaryWeapon = value; }

    public void SetPrimaryWeapon(Weapon weapon)
    {
        if (currentHolsteredMesh != null)
        {
            Destroy(currentHolsteredMesh);
        }

        foreach(Transform child in weaponHolster)
        {
            Destroy(child.gameObject);
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
        if(primaryWeaponCollider != null)
        {
            primaryWeaponCollider.EnableDamageCollider();
        }
    }

    public void DisablePrimaryDamageCollider()
    {
        if(primaryWeaponCollider != null)
        {
            primaryWeaponCollider.DisableDamageCollider();
        }
    }

    public void HolsterPrimaryWeapon()
    {
        if (currentHandMesh != null)
        {
            Destroy(currentHandMesh);
        }

        foreach (Transform child in weaponHolster)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in hand)
        {
            Destroy(child.gameObject);
        }

        if (primaryWeapon != null)
        {
            currentHolsteredMesh = Instantiate(primaryWeapon.mesh, weaponHolster);
            currentHolsteredMesh.transform.localPosition = primaryWeapon.HolsterPositionOffset;
            currentHolsteredMesh.transform.localEulerAngles = primaryWeapon.HolsterRotationOffset;
        }
        primaryWeaponCollider = null;
    }
}
