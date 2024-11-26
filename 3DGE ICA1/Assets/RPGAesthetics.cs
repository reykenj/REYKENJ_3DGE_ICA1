using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGAesthetics : MonoBehaviour
{

    [SerializeField] ProjectileWeapon projectileWeapon;
    [SerializeField] MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        UpdateRPGAesthetics(0, 0);
    }

    void OnEnable()
    {
        FPSController.OnAmmoCountChanged += UpdateRPGAesthetics;
    }

    private void UpdateRPGAesthetics(int arg1, int arg2)
    {
        if (projectileWeapon.ammoCount <= 0)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
    }

    void OnDisable()
    {
        FPSController.OnAmmoCountChanged -= UpdateRPGAesthetics;
    }
}
