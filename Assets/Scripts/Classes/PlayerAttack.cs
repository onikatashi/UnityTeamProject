using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //기본공격 쿨타임용 변수
    float nextAtkTime = 0f;

    //적이 들어있는 레이어 (monster로 설정)
    public LayerMask monsterLayer;

    void Update()
    {
        TryAttack();
    }

    /// <summary>
    /// 주변 몬스터 있는지 체크
    /// 쿨타임 체크
    /// 공격 시도
    /// </summary>
    /// <returns></returns>
    public bool TryAttack()
    {
        Player player = Player.Instance;

        float atkSpd = player.finalStats.attackSpeed;
        float atkRng = player.finalStats.attackRange;

        //주변 몬스터 탐지
        Collider[] m = Physics.OverlapSphere(
            transform.position,
            atkRng,
            monsterLayer
            );

        //몬스터가 없으면 공격 안함
        if (m.Length == 0) return false;

        // 쿨타임 체크 (쿨타임 안돌면 공격 안함
        if (Time.time < nextAtkTime) return false;

        //다음 공격 시간 설정
        nextAtkTime = Time.time + (1f / atkSpd);

        //현재 직업 가져오기
        ClassBase c = player.classStat.classLogic;
        if( c == null )
        {
            Debug.LogError("Player ClassData에 JobBase(job) 할당이 필요함.");
            return false;
        }

        //직업의 기본 공격 실행
        c.BasicAttack(player, monsterLayer);

        return true;
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
