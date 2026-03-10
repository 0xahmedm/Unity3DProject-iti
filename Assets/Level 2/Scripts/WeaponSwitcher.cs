using UnityEngine;

/// <summary>
/// Switches between weapons using scroll wheel or number keys.
/// Weapons are child GameObjects that get enabled/disabled.
/// Attach to the Player GameObject.
/// </summary>
public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapon Holder")]
    [Tooltip("Parent transform containing weapon GameObjects as children.")]
    [SerializeField] private Transform weaponHolder;

    private int currentWeaponIndex = -1;
    private int totalWeapons;

    private void Awake()
    {
        // Disable all weapons immediately so only the
        // EquipWeapon(0) call in Start triggers OnEnable.
        // Prevents the last child's OnEnable from overwriting
        // AmmoManager with wrong weapon data.
        for (int i = 0; i < weaponHolder.childCount; i++)
        {
            weaponHolder.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        totalWeapons = weaponHolder.childCount;

        if (totalWeapons == 0)
        {
            Debug.LogWarning("[WeaponSwitcher] No weapons found in holder.");
            return;
        }

        EquipWeapon(0);
    }

    private void Update()
    {
        if (totalWeapons <= 1) return;

        HandleScrollInput();
        HandleNumberKeys();
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            SwitchToNext();
        }
        else if (scroll < 0f)
        {
            SwitchToPrevious();
        }
    }

    private void HandleNumberKeys()
    {
        for (int i = 0; i < totalWeapons && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
                return;
            }
        }
    }

    private void SwitchToNext()
    {
        int next = (currentWeaponIndex + 1) % totalWeapons;
        EquipWeapon(next);
    }

    private void SwitchToPrevious()
    {
        int prev = (currentWeaponIndex - 1 + totalWeapons) % totalWeapons;
        EquipWeapon(prev);
    }

    private void EquipWeapon(int index)
    {
        if (index == currentWeaponIndex
            && weaponHolder.GetChild(index).gameObject.activeSelf)
        {
            return;
        }

        for (int i = 0; i < totalWeapons; i++)
        {
            weaponHolder.GetChild(i).gameObject.SetActive(i == index);
        }

        currentWeaponIndex = index;
    }
}
