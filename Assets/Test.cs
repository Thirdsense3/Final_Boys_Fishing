using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WayPoint
{
	public Transform point;
	public float time;
}

public class Test : MonoBehaviour 
{
	public List<WayPoint> points;

	public List<Transform> controlPoints;



	public float angle;


	public Transform target;

	public float step;

	public Transform pos0,pos1,pos2,pos3;
	public Transform pos22, pos11;
	public float time = 0;
	public float finalizedTIme;
	public Vector3 tmp;


	public Vector3 myHead;
	public float distance;
	public Vector3 direction;
	public Transform arrow;

	public float diff;
	public float normalized;

	public AnimationCurve inPut;
	public AnimationCurve release;


	public float MaxPower = 0.4f;


	public void Cast(bool cast, float power)
	{
		if(cast)
		{
			if(time<1)
				time += Time.deltaTime * 3;
			finalizedTIme = inPut.Evaluate(time);
		}
		else
		{
			finalizedTIme = release.Evaluate(time);
			if(time > 0)
				time -= Time.deltaTime * 5;
		}

	}

	void Update () 
	{


		myHead = target.position - transform.position;
		distance = myHead.magnitude;
		direction = myHead / distance; // This is now the normalized direction.
		Quaternion rotation0 = Quaternion.LookRotation(direction);
		arrow.rotation =  Quaternion.Euler(rotation0.eulerAngles + new Vector3(90,0,0));


		diff = Quaternion.Angle(transform.rotation, arrow.rotation);
		//normalized = diff/180;





		//normalized = Mathf.Lerp(0, Mathf.Abs(diff/90),finalizedTIme);
		//normalized = Mathf.Lerp(0, 1,finalizedTIme);

		normalized = Mathf.Lerp(0, MaxPower, finalizedTIme);

		Vector3 relativePos2 = target.position - pos2.position;
		Quaternion rotation2 = Quaternion.LookRotation(relativePos2);
		//pos2.rotation = Quaternion.Euler(/*rotation2.eulerAngles + */Vector3.Lerp(/*rotation2.eulerAngles + */new Vector3(0,0,0),/*rotation2.eulerAngles + */new Vector3(90,0,0), normalized));
		pos2.rotation = Quaternion.Slerp(pos22.rotation, Quaternion.Euler(rotation2.eulerAngles + new Vector3(90,0,0)), normalized);
		//pos2.localRotation = Quaternion.Euler(/*rotation2.eulerAngles + */Vector3.Lerp(/*rotation2.eulerAngles + */new Vector3(0,0,0),rotation2.eulerAngles + new Vector3(90,0,0), normalized));

		Vector3 relativePos3 = target.position - pos1.position;
		Quaternion rotation3 = Quaternion.LookRotation(relativePos3);
		//pos1.rotation = Quaternion.Euler( Vector3.Lerp(/*rotation3.eulerAngles +*/Vector3.zero, rotation3.eulerAngles + new Vector3(90,0,0), normalized));
		pos1.rotation = Quaternion.Slerp(pos11.rotation, Quaternion.Euler(rotation3.eulerAngles + new Vector3(90,0,0)), normalized);
		for(int i = 0; i<points.Count; i++)
		{
			float t0 = ((float)i+1)/(float)points.Count;
			float t1 = t0 + step;
			Vector3 pos = Mathf.Pow((1-t0),3) * pos0.position + 3 * t0 * Mathf.Pow((1 - t0),2) * pos1.position +  3 * Mathf.Pow(t0,2) * (1-t0)* pos2.position + Mathf.Pow(t0,3) * pos3.position;;
			Vector3 targetPos =Mathf.Pow((1-t1),3) * pos0.position + 3 * t1 * Mathf.Pow((1 - t1),2) * pos1.position +  3 * Mathf.Pow(t1,2) * (1-t1)* pos2.position + Mathf.Pow(t1,3) * pos3.position;;
			points[i].point.position = pos;
			Vector3 relativePos = targetPos - points[i].point.position;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			points[i].point.rotation = Quaternion.Euler(rotation.eulerAngles + new Vector3(90,0,0));
		}

		/*for(int j = 0; j<controlPoints.Count; j++)
		{
			controlPoints[j].localEulerAngles = new Vector3(angle,0,0);
		}*/
		//		for(int i = 0; i<points.Count; i++)
		//		{
		//			float t0 = ((float)i+1)/(float)points.Count;
		//			float t1 = t0 + 0.1f;
		//			Vector3 pos = Mathf.Pow((1-t0),2) * pos0.position + 2 * t0 * (1-t0) * pos1.position + Mathf.Pow(t0,2) * pos2.position;
		//			Vector3 targetPos = Mathf.Pow((1-t1),2) * pos0.position + 2 * t1 * (1-t1) * pos1.position + Mathf.Pow(t1,2) * pos2.position;
		//			points[i].point.position = pos;
		//			Vector3 relativePos = targetPos - points[i].point.position;
		//			Quaternion rotation = Quaternion.LookRotation(relativePos);
		//			points[i].point.rotation = Quaternion.Euler(rotation.eulerAngles + new Vector3(90,0,0));
		//		}
		//B.position = Mathf.Pow((1-t),2) * pos0.position + 2 * t * (1-t) * pos1.position + Mathf.Pow(t,2) * pos2.position; // ^2
		//B.position = Mathf.Pow((1-t),3) * pos0.position + 3 * t * Mathf.Pow((1 - t),2) * pos1.position +  3 * Mathf.Pow(t,2) * (1-t)* pos2.position + Mathf.Pow(t,3) * pos3.position; // 




	}
}
