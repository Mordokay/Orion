using UnityEngine;

public static class QuadraticBezierEquation
{
	static Vector3 a;
	static Vector3 b;
	static Vector3 c;

	// Method to get a point on the quadratic Bézier curve based on a specified distance
	public static void GetPointAtDistance(out Vector3 result, Vector3 p0, Vector3 p1, Vector3 p2, float distance, bool useInefficientCode)
	{
		// Initializations
		Vector3 previousPoint = p0;
		Vector3 currentPoint = Vector3.zero;
		float totalDistance = 0f;
		float step = 0.01f; // Increment for sampling along the curve
		float maxDistance = Vector3.Distance(p0, p2); // Max distance for p0 to p2

		// Sample points along the Bézier curve to find the total distance
		for (float t = 0; t <= 1; t += step)
		{
			// Get the current point on the curve
			GetCurve(out currentPoint, p0, p1, p2, t, useInefficientCode);

			// Calculate the distance from the previous point to the current point
			if (t > 0)
			{
				totalDistance += Vector3.Distance(previousPoint, currentPoint);
			}

			// If we've reached or exceeded the desired distance, return the current point
			if (totalDistance >= distance)
			{
				result = currentPoint; // Return the point at the desired distance
				return;
			}

			previousPoint = currentPoint; // Update previousPoint for the next iteration
		}

		// If the distance exceeds the length of the curve, return the end point
		result = p2; // Return the end point if distance exceeds total curve length
	}

	public static void GetCurve(out Vector3 result, Vector3 p0, Vector3 p1, Vector3 p2, float time, bool UseInefficientCode)
	{
		if (!UseInefficientCode)
		{
			Efficient(out result, p0, p1, p2, time);
		}
		else
		{
			Inefficient(out result, p0, p1, p2, time);
		}
	}

	static void Inefficient(out Vector3 result, Vector3 p0, Vector3 p1, Vector3 p2, float time)
	{
		a = Vector3.Lerp(p0, p1, time);
		b = Vector3.Lerp(p1, p2, time);
		c = Vector3.Lerp(a, b, time);
		result = c;
	}

	public static void Efficient(out Vector3 result, Vector3 p0, Vector3 p1, Vector3 p2, float time)
	{
		float tt = time * time;

		float u = 1f - time;
		float uu = u * u;

		result = uu * p0;
		result += 2f * u * time * p1;
		result += tt * p2;
	}

	public static void DrawLines(Vector3 p0, Vector3 p1, Vector3 p2, float time)
	{
		Debug.DrawLine(p0, p1, Color.green);
		Debug.DrawLine(p1, p2, Color.green);

		a = Vector3.Lerp(p0, p1, time);
		b = Vector3.Lerp(p1, p2, time);

		Debug.DrawLine(a, b, Color.yellow);
	}
}