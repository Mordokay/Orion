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

	[SerializeField] public List<GameObject> Checkpoints = new();

	protected Vector3 myPosition;

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
		List<Vector3> allPositions = new();

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

		Debug.Log("direction " + direction);

		if (direction != Vector3.zero)
		{
			Debug.Log("Add direction position " + new Vector3(direction.x * 1000f, gameObject.transform.position.y, direction.z * 1000f));
			allPositions.Add(new Vector3(direction.x * 1000f, gameObject.transform.position.y, direction.z * 1000f));
		}


		while (allPositions.Count > 0 && allPositions[allPositions.Count - 1] == Vector3.zero)
		{
			allPositions.RemoveAt(allPositions.Count - 1);
		}
		gameObject.GetComponent<LineRenderer>().SetPositions(allPositions.ToArray());
		gameObject.GetComponent<LineRenderer>().positionCount = allPositions.Count;


		//https://www.youtube.com/watch?v=L7VXcZXlhww
		//https://gamedev.stackexchange.com/questions/131108/moving-object-beyond-bezier-curve
	}

	public void RemoveTrajectory()
	{
		List<Vector3> allPositions = new();
		gameObject.GetComponent<LineRenderer>().SetPositions(allPositions.ToArray());
		gameObject.GetComponent<LineRenderer>().positionCount = allPositions.Count;
	}
}