using UnityEngine;
using static Enums;

public class PlayerMove : MonoBehaviour
{

    CharacterController cc;
    Transform cam;

    public bool canMove = true;         //외부에서 사용해야해서 public 생성

    //Player 캐싱
    Player player;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        player = Player.Instance;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!canMove)
        {
            player.animCtrl.ChangeState(PlayerAnimState.Idle);
            return;       //canMove상태가 아니면 못움직임
        }


        float mvSpd = player.finalStats.moveSpeed;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if(player != null && player.IsInputReversed)
        {
            x *= -1;
            z *= -1;
        }

        if(player != null && player.IsStunned)
        {
            x *= 0;
            z *= 0;
        }

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


        //움직일 경우 Move로 바뀌기
        if (x != 0 || z != 0)
        {
            player.animCtrl.ChangeState(PlayerAnimState.Move);
        }
        else
        {
            player.animCtrl.ChangeState(PlayerAnimState.Idle);
        }

        //공격 방향으로 sprite돌리기
        if( dir.x != 0)
        {
            player.SetFacing(dir.x);
        }
    }
}
