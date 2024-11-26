using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] public WeaponData weaponData;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private RecoilManager CameraRecoil;
    protected float nextFireTime = 0f;
    protected float ScopeTime = 10.0f;
    public int ammoCount;
    public int magazineCount = 5;
    public bool Reloading = false;
    public Coroutine ShotCoroutine;
    public bool Shooting = false;

    //protected int BurstShotCurrentIteration = 0;

    private Coroutine ReloadingCoroutine;
    // Abstract method for shooting, to be implemented by subclasses
    public abstract void StartToShoot(FPSController playerController);
    // Protected method to handle raycast logic, can be used by subclasses
    protected void PerformRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range,
        weaponData.hitLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            // Instantiate the impact effect
            GameObject Effect = Instantiate(impactEffect, hit.point,
            Quaternion.LookRotation(hit.normal));
            Destroy(Effect, 3.0f);
            if (hitObject.TryGetComponent(out Damageable damageable))
            {
                damageable.TakeDamage(weaponData.damage);
            }
        }
    }
    protected void ShootProjectile()
    {
        GameObject projectile = Instantiate(weaponData.Projectile, playerCamera.transform.position, playerCamera.transform.rotation);
        Rigidbody ProjectileRB = projectile.GetComponent<Rigidbody>();
        Projectile Proj = projectile.gameObject.GetComponent<Projectile>();
        Proj.CollisionEffectPrefab = impactEffect;
        Proj.damage = weaponData.damage;
        if (ProjectileRB != null)
        {
            ProjectileRB.velocity = playerCamera.transform.forward * weaponData.ProjectileSpeed;
        }
        Destroy(projectile, weaponData.ProjectileLifetime);
    }


    public void LookIntoScope(bool Scoping)
    {
        if (Scoping)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, weaponData.WeaponScopePosition, ScopeTime * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, ScopeTime * Time.deltaTime);
        }
    }

    public void GunRecoil()
    {
        CameraRecoil.targetRotation += new Vector3(
            weaponData.Recoil.x,
            Random.Range(-weaponData.Recoil.y, weaponData.Recoil.y),
            Random.Range(-weaponData.Recoil.z, weaponData.Recoil.z)
        );
    }

    public void StartReloading(FPSController playerController)
    {
        ReloadingCoroutine = StartCoroutine(Reload(playerController));
    }
    private IEnumerator Reload(FPSController playerController)
    {
        if (magazineCount > 0 && ammoCount < weaponData.maxAmmo)
        {
            float elapsedTime = 0.0f;
            Reloading = true;
            while (elapsedTime < weaponData.ReloadTime)
            {
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            magazineCount--;
            ammoCount = weaponData.maxAmmo;
            playerController.InvokeAmmoCountChanged();
            Reloading = false;
        }
    }

}