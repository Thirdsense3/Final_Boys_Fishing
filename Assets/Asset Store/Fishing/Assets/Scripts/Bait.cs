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
		if(BaitActive)
		{
			if(Time.time > m_StepTemp + m_currentUpdateStep)
			{
				for(int i = 0; i<TargetFishes.Count; i++)
				{
					float rnd = Random.Range(TargetFishes[i].minCatchChance, TargetFishes[i].maxCatchChance);
					if(rnd > TargetFishes[i].targetCatchChance)
					{
						GameObject m_fish = Instantiate(TargetFishes[i].fish.gameObject, transform.position, transform.rotation) as GameObject;
				
						Player.Instance.UpdateStage(PlayStage.Catching);
						Player.Instance.Spinning.CatchedFish = m_fish.GetComponent<Fish>();
						transform.SetParent(m_fish.transform);
						BaitActive = false;
					}
				}

				m_currentUpdateStep = Random.Range(minUpdateStep, maxUpdateStep);
				m_StepTemp = Time.time;
			}
		}
	}

	public void Cast(bool cast)
	{
		if(cast)
		{
			if(transform.position.y > minDeep)
				transform.Translate(Vector3.down * divingSpeedDown * Time.deltaTime);
		}
		else
		{
			if(transform.position.y < maxDeep)
				transform.Translate(Vector3.up * divingSpeedUp * Time.deltaTime);
		}
	}
}
