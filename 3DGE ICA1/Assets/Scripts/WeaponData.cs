using UnityEngine;
[CreateAssetMenu(fileName = "NewWeaponData", menuName =
"Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 100;
    public float ReloadTime = 3.0f;
    public bool SingleShot = false;
    //public bool BurstShot = false;
    public int BurstIterations = 1;
    public float BurstDelay = 0.0f;
    public Vector3 Recoil = Vector3.zero;
    public Vector3 WeaponScopePosition = Vector3.zero;
    public LayerMask hitLayers; // Layers the weapon can hit
}