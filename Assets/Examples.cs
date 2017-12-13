using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examples : MonoBehaviour {

	public Transform target;
	public Transform pos0,pos1,pos2,pos3;
	public float t0;
	public float t1;
	void Update() 
	{
		t1 = t0 + 0.1f;
		Vector3 pos = Mathf.Pow((1-t0),2) * pos0.position + 2 * t0 * (1-t0) * pos1.position + Mathf.Pow(t0,2) * pos2.position;
		Vector3 targetPos = Mathf.Pow((1-t1),2) * pos0.position + 2 * t1 * (1-t1) * pos1.position + Mathf.Pow(t1,2) * pos2.position;
		transform.position = pos;
		Vector3 relativePos = targetPos - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = rotation;
	}
}
