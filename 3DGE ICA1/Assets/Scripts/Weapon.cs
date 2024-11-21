using UnityEngine;
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] public WeaponData weaponData;
    [SerializeField] private GameObject impactEffect;
    protected float nextFireTime = 0f;

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

}