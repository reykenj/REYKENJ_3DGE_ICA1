using UnityEngine;
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] public WeaponData weaponData;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private RecoilManager CameraRecoil;
    protected float nextFireTime = 0f;
    protected float ScopeTime = 10.0f;

    public int ammoCount = 0;
    // Abstract method for shooting, to be implemented by subclasses
    public abstract void Shoot();
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

}