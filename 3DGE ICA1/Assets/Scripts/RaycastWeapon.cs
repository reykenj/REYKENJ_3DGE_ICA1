using UnityEngine;
public class RaycastWeapon : Weapon
{
    private void Start()
    {
        ammoCount = weaponData.maxAmmo;
    }
    public override void Shoot()
    {
        if (Time.time >= nextFireTime && ammoCount > 0)
        {
            nextFireTime = Time.time + weaponData.fireRate;
            PerformRaycast();
            GunRecoil();
            ammoCount -= 1;
        }
    }
}
