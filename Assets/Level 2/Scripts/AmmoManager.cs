using UnityEngine;
using System;

/// <summary>
/// Manages ammo for all weapons. Central ammo authority.
/// Other scripts query and modify ammo through this manager.
/// Attach to the Player GameObject.
/// </summary>
public class AmmoManager : MonoBehaviour
{
    /// <summary>
    /// Fired when ammo counts change. Passes
    /// (currentMagazine, maxMagazine, reserveAmmo).
    /// </summary>
    public event Action<int, int, int> OnAmmoChanged;

    private int currentMagazineAmmo;
    private int reserveAmmo;
    private WeaponData activeWeaponData;

    /// <summary>
    /// Initializes ammo counts for a weapon. Called by
    /// WeaponSwitcher when switching to a new weapon.
    /// </summary>
    public void SetWeapon(WeaponData data, int magazine, int reserve)
    {
        activeWeaponData = data;
        currentMagazineAmmo = magazine;
        reserveAmmo = reserve;
        NotifyAmmoChanged();
    }

    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int ReserveAmmo => reserveAmmo;

    /// <summary>
    /// Returns true if the magazine has ammo to fire.
    /// </summary>
    public bool CanFire()
    {
        return currentMagazineAmmo > 0;
    }

    /// <summary>
    /// Consumes one round from the magazine. Returns false
    /// if the magazine is empty.
    /// </summary>
    public bool ConsumeAmmo()
    {
        if (currentMagazineAmmo <= 0) return false;

        currentMagazineAmmo--;
        NotifyAmmoChanged();
        return true;
    }

    /// <summary>
    /// Returns true if a reload is possible (magazine not
    /// full and reserve ammo available).
    /// </summary>
    public bool CanReload()
    {
        if (activeWeaponData == null) return false;

        return currentMagazineAmmo < activeWeaponData.magazineSize
               && reserveAmmo > 0;
    }

    /// <summary>
    /// Performs the reload — moves ammo from reserve to magazine.
    /// Call this when reload animation/timer finishes.
    /// </summary>
    public void PerformReload()
    {
        if (activeWeaponData == null) return;

        int ammoNeeded = activeWeaponData.magazineSize - currentMagazineAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        currentMagazineAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;
        NotifyAmmoChanged();
    }

    /// <summary>
    /// Adds reserve ammo (from a pickup). Capped at max reserve.
    /// </summary>
    public void AddReserveAmmo(int amount)
    {
        if (activeWeaponData == null) return;

        reserveAmmo = Mathf.Min(
            reserveAmmo + amount,
            activeWeaponData.maxReserveAmmo
        );
        NotifyAmmoChanged();
    }

    private void NotifyAmmoChanged()
    {
        if (activeWeaponData == null) return;

        OnAmmoChanged?.Invoke(
            currentMagazineAmmo,
            activeWeaponData.magazineSize,
            reserveAmmo
        );
    }
}
