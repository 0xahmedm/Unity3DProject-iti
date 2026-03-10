using UnityEngine;

/// <summary>
/// ScriptableObject containing weapon configuration data.
/// Create via: Assets → Create → Weapons → Weapon Data.
/// Each weapon type gets its own asset (e.g., Phantom, Pistol).
/// </summary>
[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string weaponName = "Weapon";

    [Header("Shooting")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 10f;
    public bool isAutomatic = true;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int maxReserveAmmo = 120;
    public float reloadTime = 1.5f;
}
