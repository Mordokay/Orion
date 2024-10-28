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

	QuadraticBezierJob job;
	JobHandle jobHandle;

	Vector3 lastPosition = Vector3.zero;

	public GameObject projectile;
	[Range(0f, 1f)]
	[SerializeField] protected float mTime;
	[Range(0.1f, 10f)]
	[SerializeField] protected float timeScale;
	[SerializeField] public List<GameObject> Checkpoints = new();

	protected Vector3 myPosition;

	public void GetBezier(out Vector3 pos, List<GameObject> Checkpoints, float distance)
	{
		QuadraticBezierEquation.GetPointAtDistance(out pos,
			Checkpoints[0].transform.position,
			Checkpoints[1].transform.position,
			Checkpoints[2].transform.position,
			distance,
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
		List<Vector3> allPositions = new();

		for (float i = 0; i <= 20; i += 0.01f)
		{
			GetBezier(out myPosition, Checkpoints, i);
			if ((allPositions.Count == 0) || (myPosition != allPositions[allPositions.Count - 1]))
			{
				allPositions.Add(myPosition);
			}
		}

		gameObject.GetComponent<LineRenderer>().SetPositions(allPositions.ToArray());
		gameObject.GetComponent<LineRenderer>().positionCount = allPositions.Count;
	}

	public void RemoveTrajectory()
	{
		List<Vector3> allPositions = new();
		gameObject.GetComponent<LineRenderer>().SetPositions(allPositions.ToArray());
		gameObject.GetComponent<LineRenderer>().positionCount = allPositions.Count;
	}
}