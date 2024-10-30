using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;
using Unity.VisualScripting;

public class QuadraticBezier : MonoBehaviour
{
	[Header("Interpolate every line")]
	[SerializeField] bool UseInefficientCode;

	[Header("Jobs System")]
	[SerializeField] bool UseJobs;

	[Header("Debug")]
	[SerializeField] bool DrawDebugLines;

	[Header("Draw Full Trajectory")]
	[SerializeField] bool DrawFullTrajectory;

	[Header("The colision of the trajectory line with obscales")]
	public LayerMask collisionMask; // Set this to the layers with the colliders you want to check

	QuadraticBezierJob job;
	JobHandle jobHandle;

	Vector3 lastPosition = Vector3.zero;

	public GameObject projectile;

	[SerializeField] public List<GameObject> Checkpoints = new();

	protected Vector3 myPosition;

	public List<Vector3> allPositions = new();

	public void GetBezier(out Vector3 pos, List<GameObject> Checkpoints, float time)
	{
		QuadraticBezierEquation.GetCurve(out pos,
			Checkpoints[0].transform.position,
			Checkpoints[1].transform.position,
			Checkpoints[2].transform.position,
			time,
			UseInefficientCode);
	}

	private void Update()
	{
		if (DrawFullTrajectory && Checkpoints.Count == 3)
		{
			UpdateTrajectory();
		}
	}

	void UpdateTrajectory()
	{
		allPositions = new();

		Vector3 lastPosition = Vector3.zero;
		Vector3 direction = Vector3.zero;

		for (float i = 0; i <= 100f; i += 1f)
		{
			if (allPositions.Count == 0)
			{
				lastPosition = myPosition;
			}
			else
			{
				lastPosition = allPositions[^1];
			}

			GetBezier(out myPosition, Checkpoints, i / 100f);
			allPositions.Add(myPosition);

			direction = (myPosition - lastPosition).normalized;
		}

		if (direction != Vector3.zero)
		{
			allPositions.Add(new Vector3(direction.x * 1000f, gameObject.transform.position.y, direction.z * 1000f));
		}

		while (allPositions.Count > 0 && allPositions[allPositions.Count - 1] == Vector3.zero)
		{
			allPositions.RemoveAt(allPositions.Count - 1);
		}

		gameObject.GetComponent<LineRenderer>().SetPositions(allPositions.ToArray());
		gameObject.GetComponent<LineRenderer>().positionCount = allPositions.Count;

		CleanTrajectory();
	}

	void CleanTrajectory()
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

				// Shorten the line renderer to end at the intersection point
				GetComponent<LineRenderer>().positionCount = i + 2; // Keep positions up to current segment + 1 for hit point
				GetComponent<LineRenderer>().SetPosition(i + 1, hit.point); // Set last point to intersection

				break; // Exit the loop after finding the first hit
			}
		}
	}

	public void RemoveTrajectory()
	{
		List<Vector3> allPositions = new();
		gameObject.GetComponent<LineRenderer>().SetPositions(allPositions.ToArray());
		gameObject.GetComponent<LineRenderer>().positionCount = allPositions.Count;
	}
}