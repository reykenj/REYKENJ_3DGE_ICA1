using UnityEngine;
public class RaycastWeapon : Weapon
{
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
