using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform _target;
    public float _maxSpeed;
    [SerializeField] private float _steeringTime;
    [SerializeField] private float _lookAheadDistance;
    [SerializeField] private float _avoidDistance;
    [SerializeField][Range(0, 1)] private float _seekWeight;
    [SerializeField][Range(0, 1)] private float _ObstacleAvoidanceWeight;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 steeringVector = Seek() + ObstacleAvoidance();
        var expectedEndPostition = transform.position + Vector3.ClampMagnitude(_rigidbody.velocity + steeringVector, _maxSpeed);
        Vector3.SmoothDamp(transform.position, expectedEndPostition, ref _velocity, _steeringTime);
        _rigidbody.velocity = _velocity;
        transform.LookAt(transform.position + _rigidbody.velocity);
    }

    private Vector3 ObstacleAvoidance()
    {
        var steeringVector = Vector3.zero;
        if (!Physics.SphereCast(transform.position, 2f, _rigidbody.velocity.normalized, out RaycastHit hitInfo, _lookAheadDistance, LayerMask.GetMask("Ground")))
        {
            return steeringVector;
        }
        var avoidDirection = Vector3.Cross(transform.up, _rigidbody.velocity);
        var targetPosition = hitInfo.point + avoidDirection * _avoidDistance;
        var desiredVelocity = Vector3.ClampMagnitude(targetPosition - transform.position, _maxSpeed);
        steeringVector = desiredVelocity - _rigidbody.velocity;
        return steeringVector * _ObstacleAvoidanceWeight;
    }

    private Vector3 Seek()
    {
        var desiredVelocity = Vector3.ClampMagnitude(_target.position - transform.position, _maxSpeed);
        var steeringVector = desiredVelocity - _rigidbody.velocity;
        return steeringVector * _seekWeight;
    }
}
