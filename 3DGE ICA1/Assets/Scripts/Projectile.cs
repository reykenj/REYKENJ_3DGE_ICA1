using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    [SerializeField] private float ExplosionRadius = 1.0f;
    [SerializeField] private float ExplosionForce = 1.0f;
    public GameObject CollisionEffectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        //GameObject hitObject = collision.gameObject;
        // Instantiate the impact effect
        GameObject Effect = Instantiate(CollisionEffectPrefab, collision.contacts[0].point,
        Quaternion.LookRotation(collision.contacts[0].normal));
        Destroy(Effect, 3.0f);
        //if (hitObject.TryGetComponent(out Damageable damageable))
        //{
        //    damageable.TakeDamage(damage);
        //}
        Collider[] colliders;

        colliders = Physics.OverlapSphere(transform.position, radius: ExplosionRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];
            if (collider != null)
            {
                Rigidbody objecthit = collider.GetComponent<Rigidbody>();
                if (objecthit != null)
                {
                    Vector3 explosiondirection = (transform.position - objecthit.transform.position).normalized;
                    objecthit.AddForce(-explosiondirection * ExplosionForce, ForceMode.Impulse);
                }
                if (collider.gameObject.TryGetComponent(out Damageable damageable))
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
        Destroy(this.gameObject);
    }
}
