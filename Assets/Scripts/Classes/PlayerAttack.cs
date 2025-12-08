using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //기본공격 쿨타임용 변수
    float nextAtkTime = 0f;
    //공격 각도(범위 공격)
    public float atkAngle = 20f;

    //적이 들어있는 레이어 (monster로 설정)
    public LayerMask monsterLayer;

    // Update is called once per frame
    void Update()
    {
        TryAttack();
    }

    /// <summary>
    /// Physics.OverlapSphere를 사용
    /// 현재위치, 공격사정거리, 적레이어를 이용해서
    /// 적 위치의 +=20도를 적용하여 범위공격을 할 예정. (20도는 변경 가능)
    /// </summary>
    void BaseAttack()
    {
        //공격력
        float atkDmg = Player.Instance.finalStats.attackDamage;
        //공격 범위
        float atkRng = Player.Instance.finalStats.attackRange;


        // 1. 플레이어 중심으로 공격 범위 안에 있는 모든 Collider를 찾는다.
        Collider[] monsters = Physics.OverlapSphere(transform.position, atkRng, monsterLayer);

        // 2. 찾은 레이어를 하나씩 꺼내면서, 실제로 공격 가능한 각도/거리인지 체크
        foreach (Collider c in monsters)
        {
            //적 각도 계산
            Vector3 dirToMonster = (c.transform.position - transform.position).normalized;

            //플레이어가 보고 있는 정면 방향
            Vector3 forward = transform.forward;

            //두 벡터사이의 각도 구하기 ( 0도 = 정면 / 90도 = 옆 / 180도 = 뒤 )
            float angle = Vector3.Angle(forward, dirToMonster);

            //만약 angle이 attackAngle(20도) 이하라면 -> 전방 내에 들어온 것
            if (angle <= atkAngle)
            {
                //이 if문에 들어온 적은
                //1. 공격 거리 안에 있고,
                //2. 공격 각도 안에 있는 적

                Monster monster = c.GetComponent<Monster>();

                if (monster != null)
                {
                    monster.TakeDamage(atkDmg);
                }
                
            }
        }
    }

    /// <summary>
    /// 공격 시도하기 (쿨타임 계산)
    /// </summary>
    /// <returns></returns>
    public bool TryAttack()
    {
        //공격 속도
        float atkSpd = Player.Instance.finalStats.attackSpeed;
        //공격 범위
        float atkRng = Player.Instance.finalStats.attackRange;


        // 주위에 적이 있을 때 실행되어야 함.
        Collider[] monsters = Physics.OverlapSphere(transform.position, atkRng, monsterLayer);
        Collider nearestMonster = null;

        // 적 탐지가 안된 경우 false 반환
        if (monsters.Length == 0)
        {
            return false;
        }

        // 적 탐지가 된 경우 쿨타임 적용해서 공격
        else
        {
            //공격 쿨타임이 채워지면, 공격을 시도
            if (Time.time >= nextAtkTime)
            {
                Debug.Log("BaseAttack을 Try했다");

                nextAtkTime = Time.time + (1f / atkSpd);
                BaseAttack();
                return true;
            }
            //공격 쿨타임중엔 false
            return false;
        }
    }

    /// <summary>
    /// 범위 파악용 
    /// </summary>
    void OnDrawGizmosSelected()
    {
        //공격 범위
        float atkRng = Player.Instance.finalStats.attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRng);
    }
}
