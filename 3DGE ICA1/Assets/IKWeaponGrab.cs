using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeaponGrab : MonoBehaviour
{
    [SerializeField] IKSolver RightArm;
    [SerializeField] IKSolver LeftArm;
    [SerializeField] Vector3 RightArmOffset;
    [SerializeField] Vector3 LeftArmOffset;

    // Start is called before the first frame update
    void Start()
    {
        //RightArm.Target = transform.position + RightArmOffset;
    }
    void OnDisable()
    {
        if (RightArm != null)
        {
            RightArm.gameObject.SetActive(false);
        }
        if (LeftArm != null)
        {
            LeftArm.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (RightArm != null)
        {
            RightArm.gameObject.SetActive(true);
        }
        if (LeftArm != null)
        {
            LeftArm.gameObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (RightArm != null)
        {
            RightArm.Target = transform.TransformPoint(RightArmOffset);
        }
        if (LeftArm != null)
        {
            LeftArm.Target = transform.TransformPoint(LeftArmOffset);
        }
        //Debug.Log(transform.TransformPoint(RightArmOffset));
    }
}
