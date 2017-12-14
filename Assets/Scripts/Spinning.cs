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
        Debug.Log("Spinning Cast"); 
		Bait.transform.position = point; // 미끼의 위치가 낚시 포인트가 됨
		Line.Regenerate();
		Casted = true; // 낚시줄을 던진 상태로 활성하기 위해 
		Bait.BaitActive = true; // 미끼 활성화
		Bait.maxDeep = point.y; // 미끼의 최대 깊이
		Bait.minDeep = point.y - 10; //미끼의 최소 깊이
	}

	void Start ()
	{
		ScreenHeight = Screen.height;
		ScreenWidth = Screen.width;
	}
	float time = 0;
	void Update()
	{
        Debug.Log("Casted : " + Casted + " Catching : " + Catching);
		if(Casted) // 낚싯줄을 던진 상태일 때
		{
			Line.Regenerate();
			float Distance = Vector3.Distance(new Vector3(LineStart.position.x,Bait.transform.position.y,LineStart.position.z),Bait.transform.position);
			if(Distance<1)
			{
                Debug.Log("PlayerCasting");
				Player.UpdateStage(PlayStage.Cast); // 
				Casted = false;
			}

			Bait.Cast(Input.GetMouseButton(0));
			if(Input.GetMouseButton(0)) //왼쪽 마우스 눌렀을 때
			{
				
				Bait.transform.LookAt(new Vector3(LineStart.position.x,Bait.transform.position.y,LineStart.position.z)); //미끼가 항상 카메라쪽을 바라봄

				Bait.transform.Translate(Vector3.forward * Time.deltaTime * CastSpeed); //미끼가 앞으로 옴
			
			}
		}
		if(Catching) //Catching = 1 이면 물고기와 씨름 중
		{
			//Bait.Cast(false);
			Line.Regenerate();
			Debug.Log("Catching");
			SpinningPhysics.Cast(Input.GetMouseButton(0),power);
			if(Input.GetMouseButton(0)) //마우스 왼쪽 버튼 눌렀을 때
			{
				if(time<1)
					time += Time.deltaTime * 3; //time이 시간에 따라 증가
				CatchedFish.cathcing = true; // cathcing = true면 물고기를 당기는 상태
			}
			else // 마우스 왼쪽 버튼 뗐을 때
			{
				if(time>0)
					time -= Time.deltaTime * 3; // time이 시간에 따라 감소
				
				CatchedFish.cathcing = false; // cathcing = false면 물고기 도망가는 상태
			}
			Line.wireCatenary = Mathf.Lerp(110, 400, time); // 보간법을 이용하여 time이 

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
