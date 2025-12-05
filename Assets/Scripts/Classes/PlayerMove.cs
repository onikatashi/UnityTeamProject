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

            float mvSpd = Player.Instance.GetFinalStat().moveSpeed;

        

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0, z);
        dir.Normalize();

        
        cc.Move(dir * mvSpd * Time.deltaTime);
    }
}
