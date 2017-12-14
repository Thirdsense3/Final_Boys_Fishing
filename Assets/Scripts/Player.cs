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
        Debug.Log("Player State :" + Stage);
		switch(Stage)
		{
			case PlayStage.Idle:
				break;
			case PlayStage.Cast:
				Casting(); //낚시를 시작했을때 Casting 함수 실행
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
		if(Input.GetMouseButton(1)) //오른쪽 마우스 눌렀을 때 미끼를 던진다
		{
            Debug.Log("ThrowBait");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //마우스 커서의 위치에 ray
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) //ray 발사하여 충돌체에 부딪힐 경우
			{
				if(hit.collider.tag == "Water") //ray가 닿은 곳이 물일 경우
				{
					Marker.transform.position = hit.point; //찌의 위치가 ray의 히트 포인트
				}
			}
		}
		if(Input.GetMouseButtonUp(1)) //오른쪽 마우스 땠을 때
		{
            Debug.Log("GetRightMouseUp");
			Spinning.Cast(Marker.transform.position); // 찌의 위치가 낚시 포인트
			UpdateStage(PlayStage.Pull); // 낚시대 당기는 상태
		}
	}


}
