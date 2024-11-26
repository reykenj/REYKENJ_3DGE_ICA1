using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public GameObject CollisionEffectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        // Instantiate the impact effect
        GameObject Effect = Instantiate(CollisionEffectPrefab, collision.contacts[0].point,
        Quaternion.LookRotation(collision.contacts[0].normal));
        Destroy(Effect, 3.0f);
        if (hitObject.TryGetComponent(out Damageable damageable))
        {
            damageable.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }
}
