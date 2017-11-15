using System.Collections;
using System.Collections.Generic;
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
		float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position);
		if(cathcing)
		{
			Catching(2);
		}
		else
		{
			transform.Translate(Vector3.forward * Time.deltaTime * CurrentSpeed);

			var targetRotation = Quaternion.LookRotation(transform.position - pointer.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
			if(Time.time > m_changeDirStepTemp + CurrentDirStep)
			{
				var heading = transform.position + PlayerPos.position;
				var distance = heading.magnitude;
				var direction = heading / distance; // This is now the normalized direction.
				Vector3 pos = direction + Random.insideUnitSphere * -distance;
				pos.y = PlayerPos.position.y;
				pointer.position = pos;
				CurrentDirStep = Random.Range(ChangeDirMinStep, ChangeDirMaxStep);
				m_changeDirStepTemp = Time.time;
			}
		}
		if(distanceToPlayer < 0.1f)
		{
			transform.GetChild(1).SetParent(null);
			Player.Instance.UpdateStage(PlayStage.Cast);
			//Player.Instance.Spinning.Catching = false;
			Destroy(gameObject);
		}
	
	}

	public void Catching(float power)
	{
		if(!PlayerPos)
			return;
		transform.Translate(Vector3.forward * Time.deltaTime * power);

		var targetRotation = Quaternion.LookRotation(PlayerPos.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
		if(Time.time > m_changeDirStepTemp + CurrentDirStep)
		{
			var heading = transform.position + PlayerPos.position;
			var distance = heading.magnitude;
			var direction = heading / distance; // This is now the normalized direction.
			Vector3 pos = direction + Random.insideUnitSphere * -distance;
			pos.y = PlayerPos.position.y;
			pointer.position = pos;
			CurrentDirStep = Random.Range(ChangeDirMinStep, ChangeDirMaxStep);
			m_changeDirStepTemp = Time.time;
		}
	}
}
