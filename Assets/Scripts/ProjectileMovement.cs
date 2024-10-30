using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class ProjectileMovement : MonoBehaviour
{
    public Vector3[] positions;
    int moveIndex = 0;

    [Header("Movement Speed")]
    [SerializeField] protected float movementSpeed = 5f;

    private void Start()
    {
        transform.position = positions.First();
        moveIndex = 0;
    }

    private void Update()
    {
        Vector3 currentPos = positions[moveIndex];
        transform.position = Vector3.MoveTowards(transform.position, currentPos, movementSpeed * Time.deltaTime);

        //Rotate
        Vector3 dir = currentPos - (Vector3)transform.position;
        float angle = Mathf.Atan2(dir.normalized.x, dir.normalized.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, angle * Mathf.Rad2Deg + 90f, 0f), movementSpeed);

        float distance = Vector3.Distance(currentPos, transform.position);
        if (distance <= 0.05f)
        {
            moveIndex++;
        }

        if (moveIndex > positions.Length - 1)
        {
            Destroy(gameObject);
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