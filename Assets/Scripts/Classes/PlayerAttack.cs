using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //공격력
    public float atkDmg = Player.Instance.GetFinalStat().attackDamage;
    //공격 속도
    public float atkSpd = Player.Instance.GetFinalStat().attackSpeed;
    //다음 공격을 위한 변수
    float nextAtkTime;
    //공격 범위
    public float atkRng = Player.Instance.GetFinalStat().attackRange;
    //공격 각도(범위 공격)
    public float atkAngle = 20f;

    //적이 들어있는 레이어 (Enemy로 설정)
    public LayerMask enemyLayer;

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
        // 1. 플레이어 중심으로 공격 범위 안에 있는 모든 Collider를 찾는다.
        Collider[] enemies = Physics.OverlapSphere(transform.position, atkRng, enemyLayer);

        // 2. 찾은 레이어를 하나씩 꺼내면서, 실제로 공격 가능한 각도/거리인지 체크
        foreach( Collider c in enemies)
        {
            //적 각도 계산
            Vector3 dirToEnemy = (c.transform.position - transform.position).normalized;

            //플레이어가 보고 있는 정면 방향
            Vector3 forward = transform.forward;

            //두 벡터사이의 거리 구하기 ( 0도 = 정면 / 90도 = 옆 / 180도 = 뒤 )
            float angle = Vector3.Angle(forward, dirToEnemy);

            //만약 angle이 attackAngle(20도) 이하라면 -> 전방 내에 들어온 것
            if( angle <= atkAngle)
            {
                //이 if문에 들어온 적은
                //1. 공격 거리 안에 있고,
                //2. 공격 각도 안에 있는 적


                // 몬스터 만드는 사람한테 받아와야함!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // 몬스터 만드는 사람한테 받아와야함
                //Enemy enemy = c.GetComponent<Enemy>();
                // 몬스터 만드는 사람한테 받아와야함
                // 몬스터 만드는 사람한테 받아와야함!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                /*
                if ( enemy != null )
                {
                    enemy.TakeDamage(atkDmg);
                }
                */
            }
        }
    }

    /// <summary>
    /// 공격 시도하기 (쿨타임 계산)
    /// </summary>
    /// <returns></returns>
    public bool TryAttack()
    {
        //공격 쿨타임이 채워지면, 공격을 시도
        if(Time.time >= nextAtkTime)
        {
            nextAtkTime = Time.time + (1f / atkSpd);
            BaseAttack();
            return true;
        }
        //공격 쿨타임중엔 false
        return false;
    }


    /// <summary>
    /// 범위 파악용 
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRng);
    }
}
