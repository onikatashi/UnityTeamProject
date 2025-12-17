using Mono.Cecil.Cil;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //기본공격 쿨타임용 변수
    float nextAtkTime = 0f;

    //적이 들어있는 레이어 (monster로 설정)
    public LayerMask monsterLayer;

    //Player불러오기
    Player player;

    private void Start()
    {
        player = Player.Instance;
    }

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
        if (Time.time < nextAtkTime)
        {
            return false;
        }

        //다음 공격 시간 설정
        nextAtkTime = Time.time + (1f / atkSpd);

        //현재 직업 가져오기
        ClassBase c = player.classStat.classLogic;
        if (c == null)
        {
            return false;
        }

        Debug.Log($"[TryAttack] using class logic = {c.name}");

        Player.Instance.animCtrl.ChangeState(PlayerAnimState.Attack);
        Debug.Log("Attack상태");

        // 가까운 몬스터 Collider 찾아주고
        Collider target = FindNearestMonster(atkRng);
        if (target == null) return false;
        // 여기서 방향 확정 해주고
        Vector3 totarget = target.transform.position - player.transform.position;
        player.SetFacing(totarget.x);
        //애니메이션 넣어주기
        player.animCtrl.ChangeState(PlayerAnimState.Attack);

        //직업의 기본 공격 실행
        c.BasicAttack(player, monsterLayer);

        Debug.Log("TryAttack 실행");
        return true;
    }

    Collider FindNearestMonster(float range)
    {
        Collider[] monsters = Physics.OverlapSphere(
            transform.position,
            range,
            monsterLayer
        );

        float minDist = Mathf.Infinity;
        Collider nearest = null;

        foreach (var m in monsters)
        {
            float d = (m.transform.position - transform.position).sqrMagnitude;
            if (d < minDist)
            {
                minDist = d;
                nearest = m;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 범위 파악용 
    /// </summary>
    void OnDrawGizmosSelected()
    {
        //공격 범위
        float atkRng = player.finalStats.attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRng);
    }
}
