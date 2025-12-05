using UnityEngine;

public class PlayerMove : MonoBehaviour
{


    CharacterController cc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        //Player 데이터에서 호출한 총합 MoveSpeed를 사용한 Move
        float mvSpd = Player.Instance.GetFinalStat().moveSpeed;

        

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0, z);
        dir.Normalize();

        
        cc.Move(dir * mvSpd * Time.deltaTime);
    }
}
