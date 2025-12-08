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

        // 가장 가까이 있는 몬스터를 저장해둘 변수
        Collider nearest = null;

        // 지금까지 찾은 몬스터들 중에서 최소 거리를 저장하는 변수
        // 처음에는 어떤 몬스터의 거리든 이 값보다 작게 만들기 위해 엄청나게 큰 값으로 초기화해준다.
        // 처음에는 무한대라고 설정해두고, 실제 거리값이 들어오면 무조건 이 값보다 작기 때문에
        // '첫' 몬스터가 일단 '가장 가까운' 몬스터가 된다.
        float minDist = Mathf.Infinity;

        foreach (Collider m in monsters)
        {
            // 몬스터와의 거리, sqrMagnitude 사용
            float dist = (transform.position - m.transform.position).sqrMagnitude;

            // 만약 지금 계산한 거리(dist)가 지금까지 기록해둔 최소 거리 (minDist)보다 더 작다면,
            // -> 이 몬스터가 현재까지 본 몬스터들 중 가장 가깝다
            if (dist < minDist)
            {
                //minDist를 이 몬스터까지의 거리로 갱신
                minDist = dist;

                //가장 가까운 몬스터(변수)를 현재 이 몬스터로 바꿔줌
                nearest = m;
            }
        }

        // 2. 기준 방향 설정
        // 방향만 필요하니까 normalized를 씀
        Vector3 centerDir = (nearest.transform.position - transform.position).normalized;

        // 3. 기준 방향 +-20도 안의 모든 적 공격
        foreach (Collider c in monsters)
        {
            //방향 벡터 노멀라이즈
            Vector3 dir = (c.transform.position - transform.position).normalized;

            // Vector3.Angle(a, b)
            //두 방향 벡터 a, b 사이의 각도를 0~ 180도 사이의 float 값으로 반환
            float angle = Vector3.Angle(centerDir, dir);

            // 이 몬스터가 가장 가까운 몬스터 방향에서 몇도 옆으로 떨어져있냐
            // 20도 이하로 떨어져있다면,
            if (angle <= atkAngle)
            {
                //
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
