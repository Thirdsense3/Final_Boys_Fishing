using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour 
{	
	public Player Player;
	public Line Line;
	public Bait Bait;

	public Fish CatchedFish;

	public Transform LineStart;

	public bool Casted = false;

	public bool Catching = false;

	public float CastSpeed = 0.5f;

	[Header("SpinningSettings")]
	public float power;
	public Test SpinningPhysics;


	private float ScreenWidth;
	private float CursorWidth;
	private float NormalizedWidth;
	[Header("SpinningContol")]
	public float MaxXValue;
	public float MinXValue;


	private float ScreenHeight;
	private float CursorHeight;
	private float NormalizedHeight;

	public float MaxYValue;
	public float MinYValue;

	public void Cast( Vector3 point )
	{
		Bait.transform.position = point;
		Line.Regenerate();
		Casted = true;
		Bait.BaitActive = true;
		Bait.maxDeep = point.y;
		Bait.minDeep = point.y - 10;
	}

	void Start ()
	{
		ScreenHeight = Screen.height;
		ScreenWidth = Screen.width;
	}
	float time = 0;
	void Update()
	{
		if(Casted)
		{
			Line.Regenerate();
			float Distance = Vector3.Distance(new Vector3(LineStart.position.x,Bait.transform.position.y,LineStart.position.z),Bait.transform.position);
			if(Distance<1)
			{
				Player.UpdateStage(PlayStage.Cast);
				Casted = false;
			}

			Bait.Cast(Input.GetMouseButton(0));
			if(Input.GetMouseButton(0))
			{
				
				Bait.transform.LookAt(new Vector3(LineStart.position.x,Bait.transform.position.y,LineStart.position.z));

				Bait.transform.Translate(Vector3.forward * Time.deltaTime * CastSpeed);
			
			}
		}
		if(Catching)
		{
			//Bait.Cast(false);
			Line.Regenerate();
			Debug.Log("Catching");
			SpinningPhysics.Cast(Input.GetMouseButton(0),power);
			if(Input.GetMouseButton(0))
			{
				if(time<1)
					time += Time.deltaTime * 3;
				CatchedFish.cathcing = true;
			}
			else
			{
				if(time>0)
					time -= Time.deltaTime * 3;
				
				CatchedFish.cathcing = false;
			}
			Line.wireCatenary = Mathf.Lerp(110, 400, time);

		}
		else
		{
			SpinningPhysics.Cast(false,power);
		}
		RotateSpinning();
	}

	public void RotateSpinning()
	{
		CursorHeight = Mathf.Clamp(Input.mousePosition.y, 0, ScreenHeight); 
		CursorWidth = Mathf.Clamp(Input.mousePosition.x, 0, ScreenWidth);

		NormalizedHeight = CursorHeight / ScreenHeight;
		NormalizedWidth = CursorWidth / ScreenWidth;

		transform.localRotation = Quaternion.Euler(Mathf.Lerp(MinYValue, MaxYValue, NormalizedHeight), Mathf.Lerp(MinXValue, MaxXValue, NormalizedWidth), 0);
	}




}
