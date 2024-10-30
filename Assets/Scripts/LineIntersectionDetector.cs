using UnityEngine;
using System.Collections.Generic;

public class LineIntersectionDetector : MonoBehaviour
{
    public LayerMask collisionMask; // Set this to the layers with the colliders you want to check

    void Update()
    {
        Vector3? firstHitPoint = null;

        // Loop through each segment of the line
        for (int i = 0; i < GetComponent<LineRenderer>().positionCount - 1; i++)
        {
            Vector3 startPoint = GetComponent<LineRenderer>().GetPosition(i);
            Vector3 endPoint = GetComponent<LineRenderer>().GetPosition(i + 1);
            Vector3 direction = (endPoint - startPoint).normalized;
            float segmentLength = Vector3.Distance(startPoint, endPoint);

            // Raycast from startPoint to endPoint
            if (Physics.Raycast(startPoint, direction, out RaycastHit hit, segmentLength, collisionMask))
            {
                firstHitPoint = hit.point;
                Debug.Log("First intersection detected at: " + hit.point + " totalcount: " + GetComponent<LineRenderer>().positionCount + " index: " + i);

                // Shorten the line renderer to end at the intersection point
                GetComponent<LineRenderer>().positionCount = i + 2; // Keep positions up to current segment + 1 for hit point
                GetComponent<LineRenderer>().SetPosition(i + 1, hit.point); // Set last point to intersection

                break; // Exit the loop after finding the first hit
            }
        }
    }
}