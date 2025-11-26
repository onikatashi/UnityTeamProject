using UnityEngine;

public class PlayerMove : MonoBehaviour
{ 
    float speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // 상하 이동 장운이형
        float moveY=Input.GetAxis("Vertical");

        transform.Translate(0f, moveY * speed * Time.deltaTime,0f);

        // 좌우 이동 의현이
    }
}
