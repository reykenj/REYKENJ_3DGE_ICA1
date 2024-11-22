using UnityEngine;

public class IKSolver : MonoBehaviour
{
    [SerializeField] private Transform _pivot, _upper, _lower, _tip;
    public Vector3 Target;
    /*[SerializeField] */
    private float _upperLength, _lowerLength;
    //public bool Grounded = false;

    private void Start()
    {
        // Calculate lengths of the upper and lower segments
        _upperLength = (_lower.position - _upper.position).magnitude;
        _lowerLength = (_tip.position - _lower.position).magnitude;
        //_lower.transform.position = transform.TransformPoint(new Vector3(0,0, _upperLength));
    }

    private void Update()
    {
        // Direct the pivot to look at the target
        _pivot.transform.LookAt(Target);

        var a = _upperLength;
        var b = _lowerLength;
        var c = (Target - _upper.position).magnitude;

        // Calculate angles using the law of cosines
        var alpha = Mathf.Acos((a * a + c * c - b * b) / (2 * a * c)) * Mathf.Rad2Deg;
        var gamma = Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)) * Mathf.Rad2Deg;

        // If the angles are valid, adjust the joint rotations
        if (!float.IsNaN(alpha) && !float.IsNaN(gamma))
        {
            var beta = 180 - gamma;

            // Rotate the upper joint towards the target
            _upper.localRotation = Quaternion.AngleAxis(alpha, Vector3.left);

            // Rotate the lower joint relative to the upper joint
            _lower.localRotation = Quaternion.AngleAxis(-beta, Vector3.left);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the target position as a sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Target, 0.1f);
    }
}
