using System.Collections;
using UnityEngine;
public class RaycastWeapon : Weapon
{
    private void Start()
    {
        ammoCount = weaponData.maxAmmo;
    }
    public override void StartToShoot(FPSController playerController)
    {
        ShotCoroutine = StartCoroutine(Shoot(playerController));
    }
    private IEnumerator Shoot(FPSController playerController)
    {
        int BurstShotCurrentIteration = 0;
        while (BurstShotCurrentIteration < weaponData.BurstIterations)
        {
            Shooting = true;
            if (Time.time >= nextFireTime && ammoCount > 0)
            {
                nextFireTime = Time.time + weaponData.fireRate;
                PerformRaycast();
                GunRecoil();
                ammoCount -= 1;
                BurstShotCurrentIteration++;
                playerController.InvokeAmmoCountChanged();
            }
            else if(ammoCount <= 0)
            {
                break;
            }

            yield return null;
        }
        float elapsedTime = 0.0f;
        while (elapsedTime < weaponData.BurstDelay)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        Shooting = false;

    }
}
