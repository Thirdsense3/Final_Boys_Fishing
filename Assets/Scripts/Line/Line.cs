using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class Line : Catenary {
	private LineRenderer lineRenderer;
	public int MaxVisiblePoints = 30;
	// Use this for initialization
	void Start () 
	{
		lineRenderer = this.GetComponent<LineRenderer>();
	}
	
	public override void Regenerate ()
	{
		float distance = Vector3.Distance (p1.position, p2.position);
		int nPoints = (int) (distance/wireResolution + 1);

		//wireResolution = distance / (nPoints-1);

		Vector3[] wirePoints = new Vector3[nPoints];
		wirePoints[0] = p1.position;
		wirePoints[nPoints-1] = p2.position;

		Vector3 dir = (p2.position - p1.position).normalized;
		float offset = calculateCatenary( wireCatenary, -distance/2 );

		for (int i = 1; i < nPoints - 1; ++i) {
			Vector3 wirePoint = p1.position + i * wireResolution * dir;

			float x = i * wireResolution - distance / 2;
			wirePoint.y = wirePoint.y - (offset - calculateCatenary (wireCatenary, x));

			wirePoints[i] = wirePoint;
		}
		lineRenderer.positionCount = nPoints;
		lineRenderer.SetPositions(wirePoints);
	}
}
