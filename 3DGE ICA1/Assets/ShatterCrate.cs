using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterCrate : MonoBehaviour
{
    [SerializeField] GameObject ShatterCratePrefab;
    [SerializeField] Damageable damageable;

    private bool Broken = false;

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] BoxCollider boxCollider;
    GameObject ShatterCrateObject;
    [SerializeField] float DisappearTimeMax = 3.0f;
    private float DisappearTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Broken)
        {
            DisappearTime += Time.deltaTime;
            if (DisappearTime >= DisappearTimeMax)
            {
                foreach (Transform child in ShatterCrateObject.transform)
                {
                    child.localScale = Vector3.Lerp(child.localScale, Vector3.zero, Time.deltaTime);
                }
                if (ShatterCrateObject.transform.GetChild(0).localScale.x < 0.1)
                {
                    Destroy(ShatterCrateObject);
                    Destroy(this.gameObject);
                }
            }
        }
        else if (damageable.health <= 0)
        {
            ShatterCrateObject = Instantiate(ShatterCratePrefab, transform.position, transform.rotation);
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
            damageable.enabled = false;
            Broken = true;
            foreach (Transform child in ShatterCrateObject.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                rb.AddExplosionForce(100.0f, ShatterCrateObject.transform.position, 10.0f);
            }
            //Destroy(Effect, 3.0f);
            //Destroy(this.gameObject);
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
