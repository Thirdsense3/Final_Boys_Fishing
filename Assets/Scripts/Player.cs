using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayStage{Idle, Cast, Pull, Catching, Catched}

public class Player : MonoBehaviour 
{
	public static Player Instance;

	public PlayStage Stage;
	public Spinning Spinning;

	public Marker Marker;

	void Start()
	{
		Instance = this;
	}

	void Update () 
	{
		switch(Stage)
		{
			case PlayStage.Idle:
				break;
			case PlayStage.Cast:
				Casting();
			Spinning.Catching = false;
			Spinning.Bait.Cast(false);
				break;
		case PlayStage.Catching:
			Spinning.Casted = false;
			Spinning.Catching = true;
			break;
		}
	}

	public void UpdateStage(PlayStage newStage)
	{
		Stage = newStage;
	}

	void Casting()
	{
		if(Input.GetMouseButton(1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.tag == "Water")
				{
					Marker.transform.position = hit.point;
				}
			}
		}
		if(Input.GetMouseButtonUp(1))
		{
			Spinning.Cast(Marker.transform.position);
			UpdateStage(PlayStage.Pull);
		}
	}


}
