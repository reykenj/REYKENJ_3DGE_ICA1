using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] GameObject ExplosiveEffect;
    [SerializeField] Damageable damageable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (damageable.health <= 0)
        {
            GameObject Effect = Instantiate(ExplosiveEffect, transform.position, transform.rotation);
            Destroy(Effect, 3.0f);
            Destroy(this.gameObject);
        }
    }

    private void OnDisable()
    {
        //Destroy(gameObject);
    }

    private void Instantiate(GameObject explosiveEffect, Vector3 position, Vector3 zero)
    {
        throw new NotImplementedException();
    }
}
