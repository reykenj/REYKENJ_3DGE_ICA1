using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] GameObject ExplosiveEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Instantiate(ExplosiveEffect, transform.position, Vector3.zero);
        Destroy(ExplosiveEffect, 3.0f);
        //Destroy(gameObject);
    }

    private void Instantiate(GameObject explosiveEffect, Vector3 position, Vector3 zero)
    {
        throw new NotImplementedException();
    }
}
