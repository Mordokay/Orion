using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;
using System.Collections.Generic;

public class ProjectileMovement : MonoBehaviour
{
    [Header("Interpolate every line")]
    [SerializeField] bool UseInefficientCode;

    [Header("Jobs System")]
    [SerializeField] bool UseJobs;

    QuadraticBezierJob job;
    JobHandle jobHandle;

    Vector3 lastPosition = Vector3.zero;

    [Range(0f, 1f)]
    [SerializeField] protected float mTime;

    protected Vector3 myPosition;

    [Header("Movement Speed")]
    [SerializeField] protected float movementSpeed = 5f; // Speed in units per second

    private float totalDistance; // Total distance between the checkpoints

    public Vector3 finalPlayerPosition = Vector3.zero;
    public Vector3 finalMouseDownPosition = Vector3.zero;
    public Vector3 finalMouseDragPosition = Vector3.zero;

    public void GetBezier(out Vector3 pos, float time)
    {
        QuadraticBezierEquation.GetCurve(out pos,
            finalPlayerPosition,
            finalMouseDownPosition,
            finalMouseDragPosition,
            time,
            UseInefficientCode);
    }

    private void Start()
    {
        totalDistance = 0f; // Reset total distance for new calculations

        for (int i = 0; i <= 1000; i++)
        {
            if (i == 0)
            {
                lastPosition = finalPlayerPosition;
            }

            GetBezier(out myPosition, i / 1000.0f);

            // Calculate the distance from the last position to the current position
            if (i > 0)
            {
                totalDistance += Vector3.Distance(lastPosition, myPosition);
            }
            lastPosition = myPosition;
        }
    }

    private void Update()
    {
        // Calculate the distance moved based on speed
        float distanceToMove = movementSpeed * Time.deltaTime;

        // Calculate the new mTime based on distance and total distance
        if (totalDistance > 0)
        {
            // Increment mTime proportionally to the distance moved
            mTime += distanceToMove / totalDistance;
        }

        UpdateProjectile();
    }

    void UpdateProjectile()
    {
        // jobs approach
        if (UseJobs)
        {
            // you cannot modify values inside struct returned from another component
            // bezier curve (result position) is calculated inside the struct when job is executed
            // in order to get the result you need native arrays which is the reference
            NativeArray<Vector3> result = new NativeArray<Vector3>(1, Allocator.TempJob);

            job = new QuadraticBezierJob()
            {
                p0 = finalPlayerPosition,
                p1 = finalMouseDownPosition,
                p2 = finalMouseDragPosition,
                time = mTime,
                useInefficientCode = UseInefficientCode,
                resultArray = result
            };

            jobHandle = job.Schedule();
            jobHandle.Complete();
            gameObject.transform.position = result[0];

            // clean up unmanaged memory & prevent memory leak
            // native containers won't be handled by garbage collector
            result.Dispose();
        }

        // traditional approach
        else
        {
            GetBezier(out myPosition, mTime);
            gameObject.transform.position = myPosition;
        }
    }

    // This method will be called when the projectile collides with another object
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is on the "TargetLayer"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // Handle collision with the target layer
            Debug.Log("Hit target: " + collision.gameObject.name);
            // You can add additional logic here, such as destroying the projectile or triggering an effect
            Destroy(gameObject); // Destroy the projectile upon collision (optional)
        }
    }
}