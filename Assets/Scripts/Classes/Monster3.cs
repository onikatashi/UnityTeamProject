using UnityEngine;
/// <summary>
/// 일단 자폭 패턴
/// </summary>
public class Monster3 : MonsterBase
{
    [Header("자폭 설정")]
    public float explosionRanege = 3f;     // 폭발 범위
    public float explosionDamage = 50f;    // 폭발 피해

    protected override void Idle()
    {
        agent.isStopped = true;

        if (player == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);
        if (dis <= detectRange)
        {
            state = Enums.MonsterState.Move;
            anim.SetTrigger("Move");
        }
    }

    protected override void Move()
    {
        if (player == null || md == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        // 사거리 밖이면 계속 추적
        if (dis > md.attackRange)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // 사거리 안에 들어왔으면 자폭 준비
            agent.isStopped = true;
            agent.updateRotation = false;

            state = Enums.MonsterState.Attack;
            timer = 0f; 
            anim.SetTrigger("Attack");  // 자폭 준비 모션
        }
    }

    protected override void Attack()
    {
        if (player == null || md == null) return;

        agent.isStopped = true;
        agent.updateRotation = false;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        //플레이어가 너무 멀리 도망가면 자폭 취소하고 다시 Move로 돌아갈 수도 있음
        if (dis > md.attackRange * 1.5f)
        {
            state = Enums.MonsterState.Move;
            agent.updateRotation = true;
            anim.SetTrigger("Move");
            timer = 0f;
            return;
        }

        // 딜레이 타이머 (md.attackSpeed만큼 대기 후 자폭)
        timer += Time.deltaTime;
        if (timer < md.attackSpeed) return;

        Explode();
    }

    private void Explode()
    {
        // 폭발 애니메이션 (수정할수도 있음)
        anim.SetTrigger("Explode");

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRanege); // OverlapSphere -> 범위 안에 있는 오브젝트 감지하는 함수
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                
                var hp = hit.GetComponent<Player>(); // 수정하기
                if (hp != null)
                {
                    hp.TakeDamage(explosionDamage);
                }
            }
        }

        // 자폭 후 몬스터 사망
        state = Enums.MonsterState.Die;
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRanege);
    }
}
