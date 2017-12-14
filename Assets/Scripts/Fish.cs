using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Fish : MonoBehaviour 
{
	public Transform pointer;

	[Header("Battle Setting")]
	public Transform PlayerPos;

	public float StaminaMax;
	public float Stamina;
	public float StaminaRegen;

	public float MinBoostEnable;
	public float MaxBoostEnable;
	public float CurrentBoostTemp;

	public float CurrentSpeed;

	public float BoostSpeed;
	public float RegularSpeed;

	public float RotationSpeed;

	public float ChangeDirMinStep;
	public float ChangeDirMaxStep;
	public float CurrentDirStep;

	private float m_changeDirStepTemp;

	public bool cathcing = false;

	// Use this for initialization
	void Start () 
	{
		PlayerPos = Player.Instance.transform;
		pointer.SetParent(null);
	}
	
	// Update is called once per frame
	void Update ()
	{
        Debug.Log("Activate Fish"); //물고기 활성
		float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position); //플레이어와의 거리
		if(cathcing) //cathcing = true 이면 물고기랑 씨름 중일 때
		{
			Catching(2); // 당기는 힘을 2로 줄임 (물고기가 물고 있으니까)
		}
		else // cathcing = false 이면 물고기가 도망가는 중
		{
			transform.Translate(Vector3.forward * Time.deltaTime * CurrentSpeed); // 물고기가 바라보는 방향으로 도망감

			var targetRotation = Quaternion.LookRotation(transform.position - pointer.position); // 물고기가 바라보는 방향을 포인터의 반대방향으로!
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime); // 물고기가 targetRotation 방향으로 회전. 속도는 시간비례
			if(Time.time > m_changeDirStepTemp + CurrentDirStep)
			{
				var heading = transform.position + PlayerPos.position;
				var distance = heading.magnitude;
				var direction = heading / distance;
				Vector3 pos = direction + Random.insideUnitSphere * -distance;
				pos.y = PlayerPos.position.y;
				pointer.position = pos;
				CurrentDirStep = Random.Range(ChangeDirMinStep, ChangeDirMaxStep); // 미리 설정해놓은 min ~ max 사이에 랜덤하게 설정
				m_changeDirStepTemp = Time.time;
			}
		}
		if(distanceToPlayer < 0.1f) //플레이어와의 거리가 최대로 좁혀졌을 때
		{
            Debug.Log("물고기 잡았다!");
            
			transform.GetChild(1).SetParent(null);
			Player.Instance.UpdateStage(PlayStage.Cast); // 물고기를 잡았으니 다시 낚시 시작 상태로 돌아가기
			//Player.Instance.Spinning.Catching = false;
			Destroy(gameObject); //물고기 없애버리기
            SceneManager.LoadScene("MainGame"); //메인 씬으로 돌아가기
            //Instantiate(TargetFishes[i].fish.gameObject, transform.position, transform.rotation) as GameObject;
		}
	
	}

	public void Catching(float power)
	{
		if(!PlayerPos)
			return;
		transform.Translate(Vector3.forward * Time.deltaTime * power); // 물고기가 앞으로 당겨옴

		var targetRotation = Quaternion.LookRotation(PlayerPos.position - transform.position); // 물고기가 카메라쪽 방향을 바라보게 회전
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime); // 물고기가 targetRotation 방향으로 회전 속도 시간 비례
		if(Time.time > m_changeDirStepTemp + CurrentDirStep)
		{
			var heading = transform.position + PlayerPos.position;
			var distance = heading.magnitude;
			var direction = heading / distance;
			Vector3 pos = direction + Random.insideUnitSphere * -distance;
			pos.y = PlayerPos.position.y;
			pointer.position = pos;
			CurrentDirStep = Random.Range(ChangeDirMinStep, ChangeDirMaxStep);
			m_changeDirStepTemp = Time.time;
		}
	}
}
