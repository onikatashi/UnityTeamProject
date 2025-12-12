using Unity.Mathematics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    CharacterController cc;
    Transform cam;

    public bool canMove = true;         //외부에서 사용해야해서 public 생성

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!canMove) return;       //canMove상태가 아니면 못움직임

        float mvSpd = Player.Instance.finalStats.moveSpeed;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //카메라 기준 방향 계산
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        //위 / 아래 각도 무시
        camForward.y = 0f;  
        camRight.y = 0f;

        //노멀라이즈
        camForward.Normalize();
        camRight.Normalize();

        //카메라 기준 이동
        Vector3 dir = camForward * z + camRight * x;

        cc.Move(dir * mvSpd * Time.deltaTime);
    }
}
