using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {


    public bool left;

    float speed;


    void Awake()
    {
        speed = Random.Range(1, 4);
    }

	void Update () {

        if (left)
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        else
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }

        if (transform.position.x < -10 || transform.position.x > 10)
        {
            left = Random.Range(0, 2) == 0;

            speed = Random.Range(1, 4);

            Vector3 v = transform.position;

            if (left)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                v.x = 10;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
                v.x = -10;
            }

            transform.position = v;

        }

	}
}
