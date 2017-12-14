using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FishCatchChance
{
	public Fish fish;
	public float minCatchChance;
	public float maxCatchChance;

	public float targetCatchChance;

}

public class Bait : MonoBehaviour
{

	public bool BaitActive = false;

	public float Catchability;
	public List<FishCatchChance> TargetFishes;

	public float minUpdateStep;
	public float maxUpdateStep;

	private float m_currentUpdateStep;
	private float m_StepTemp;

	public float minDeep;
	public float maxDeep;
	public float divingSpeedDown;
	public float divingSpeedUp;

	// Update is called once per frame
	void Update () 
	{
		if(BaitActive) //미끼가 활성화 됐을 때
		{
			if(Time.time > m_StepTemp + m_currentUpdateStep) 
			{
				for(int i = 0; i<TargetFishes.Count; i++) // 물고기의 수만큼 실행 (여기서는 한마리)
				{
					float rnd = Random.Range(TargetFishes[i].minCatchChance, TargetFishes[i].maxCatchChance); // rnd 값이 계속 min ~ max 사이에 랜덤으로 정해짐
					if(rnd > TargetFishes[i].targetCatchChance) // 정해진 CatchChance 값보다 rnd가 커지면 물고기가 생성(미끼를 뭄)
					{
						GameObject m_fish = Instantiate(TargetFishes[i].fish.gameObject, transform.position, transform.rotation) as GameObject; // instantiate 이용하여 물고기 생성
				
						Player.Instance.UpdateStage(PlayStage.Catching); // 플레이어 상태를 Catching 상태로
						Player.Instance.Spinning.CatchedFish = m_fish.GetComponent<Fish>();
						transform.SetParent(m_fish.transform);
						BaitActive = false; // 미끼 비활성화
					}
				}

				m_currentUpdateStep = Random.Range(minUpdateStep, maxUpdateStep); // min 값과 max 값 사이에 랜덤하게 계속 값 정함
				m_StepTemp = Time.time;
			}
		}
	}

	public void Cast(bool cast)
	{
		if(cast)
		{
			if(transform.position.y > minDeep) // 최소 깊이 이상으로 물에 잠김
				transform.Translate(Vector3.down * divingSpeedDown * Time.deltaTime);
		}
		else
		{
			if(transform.position.y < maxDeep) // 최대 깊이 이하로 물에 잠김
				transform.Translate(Vector3.up * divingSpeedUp * Time.deltaTime);
		}
	}
}
