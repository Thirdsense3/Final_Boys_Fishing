using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.C))
            {
                SceneManager.LoadScene("Fishing2");
            }
        }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Boat")
        {
            SceneManager.LoadScene("Fishing2");
        }
    }
}
