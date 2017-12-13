using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour 
{
	public Transform Target;
	public float smoothSpeed = 2;

	/*void Update () 
	{
		if((transform.position - Target.position).sqrMagnitude > 0.1f)
		{
			Smooth();
		}
	}

	public void Smooth()
	{
		Debug.Log("Smooth");
		float step = smoothSpeed * Time.deltaTime;
		transform.position = Vector3.Lerp(transform.position, Target.position, step);
	}*/
}
