using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ProjectileMovement : MonoBehaviour
{
    [Header("Interpolate every line")]
    [SerializeField] bool UseInefficientCode;

    [Header("Jobs System")]
    [SerializeField] bool UseJobs;

    QuadraticBezierJob job;
    JobHandle jobHandle;

    [Range(0f, 1f)]
    [SerializeField] protected float mTime;

    protected Vector3 lastPosition = Vector3.zero;
    protected Vector3 myPosition;
    protected Vector3 direction;

    [Header("Movement Speed")]
    [SerializeField] protected float movementSpeed = 5f; // Speed in units per second

    private float totalDistance; // Total distance between the checkpoints
    private bool useLinearDirection = false;

    public Vector3 finalPlayerPosition = Vector3.zero;
    public Vector3 finalMouseDownPosition = Vector3.zero;
    public Vector3 finalMouseDragPosition = Vector3.zero;

    public void GetBezier(out Vector3 pos, float distance)
    {
        QuadraticBezierEquation.GetPointAtDistance(out pos,
            finalPlayerPosition,
            finalMouseDownPosition,
            finalMouseDragPosition,
            distance,
            UseInefficientCode);
    }

    private void Start()
    {

    }

    private void Update()
    {
        // Calculate the distance moved based on speed
        float distanceToMove = movementSpeed * Time.deltaTime;

        mTime += distanceToMove;

        UpdateProjectile(distanceToMove);
    }

    void UpdateProjectile(float distanceToMove)
    {
        if (useLinearDirection)
        {
            gameObject.transform.position += direction * distanceToMove * 10;
        }
        else if (UseJobs)
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

            lastPosition = gameObject.transform.position;
            gameObject.transform.position = result[0];
            direction = (result[0] - lastPosition).normalized;

            if (Vector3.Distance(gameObject.transform.position, finalMouseDragPosition) < 0.1f)
            {
                useLinearDirection = true;
            }
            // clean up unmanaged memory & prevent memory leak
            // native containers won't be handled by garbage collector
            result.Dispose();
        }
        else
        {
            GetBezier(out myPosition, mTime);
            lastPosition = gameObject.transform.position;
            gameObject.transform.position = myPosition;
            direction = (myPosition - lastPosition).normalized;

            Debug.Log("Distance: " + Vector3.Distance(gameObject.transform.position, finalMouseDragPosition));
            if (Vector3.Distance(gameObject.transform.position, finalMouseDragPosition) < 0.1f)
            {
                useLinearDirection = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}