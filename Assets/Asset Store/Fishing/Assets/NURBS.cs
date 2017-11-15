using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NURBS : MonoBehaviour
{
	public List<Transform> P;
	public Vector3 B;

	public int n;

	public List<Transform> K;
	public Vector3 V;


	public Transform Cube;

	// Use this for initialization
	void Start () {
		
	}

	public void Click()
	{
		B = P[n+1].position;
		V = ((K[n+3].position - K[n+2].position).magnitude / (K[n+3].position - K[n+1].position).magnitude) * P[n].position + ((K[n+2].position - K[n+1].position).magnitude / (K[n+3].position - K[n+1].position).magnitude) * P[n+1].position;
		n++;
		Cube.position = V;
	}

	// Update is called once per frame
	/*void Update () 
	{
		for(int n = 0; n<P.Count-3; n++)
		{
		//n = Mathf.Clamp(n,0,P.Count - 4);
			B = P[n+1].position;
			V = ((K[n+3].position - K[n+2].position).magnitude / (K[n+3].position - K[n+1].position).magnitude) * P[n].position + ((K[n+2].position - K[n+1].position).magnitude / (K[n+3].position - K[n+1].position).magnitude) * P[n+1].position;
		}
	}*/
}
