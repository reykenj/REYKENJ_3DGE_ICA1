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

    // Update is called once per frame
    void Update()
    {
        RightArm.Target = transform.TransformPoint(RightArmOffset);
        //Debug.Log(transform.TransformPoint(RightArmOffset));
    }
}
