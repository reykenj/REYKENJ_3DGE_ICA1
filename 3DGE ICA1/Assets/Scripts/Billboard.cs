using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] float Height = 0.3f;
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = transform.parent.InverseTransformDirection(Vector3.up) * Height;
        Quaternion rotation = Camera.main.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
}