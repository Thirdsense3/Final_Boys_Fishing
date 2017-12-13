using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // c 키 누르면 씬 변경
        {
            SceneManager.LoadScene("Fishing2");

            Cursor.lockState = CursorLockMode.None; // 마우스 커서 다시 보이게 하기
            Cursor.visible = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Boat") // 캐릭터가 보트와 충돌하면 씬 변경
        {
            SceneManager.LoadScene("Fishing2");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (collision.transform.tag == "Rabbit")
        {
            Debug.Log("collision");
            Application.Quit();
        }
    }
}
