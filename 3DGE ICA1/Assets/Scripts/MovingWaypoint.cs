using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWaypoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform Waypoint1;
    [SerializeField] Transform Waypoint2;

    private Transform CurrentWaypoint;
    [SerializeField] Boid boid;
    [SerializeField] float MinDistance = 3.0f;
    void Start()
    {
        CurrentWaypoint = boid._target;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, CurrentWaypoint.position);
        if(distance <= MinDistance)
        {
            if (CurrentWaypoint != Waypoint1)
            {
                CurrentWaypoint = Waypoint1;
            }
            else
            {
                CurrentWaypoint = Waypoint2;
            }
            boid._target = CurrentWaypoint;
        }
    }
}
