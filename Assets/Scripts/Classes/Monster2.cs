using UnityEngine;
/// <summary>
/// 원거리 몬스터 패턴1
/// </summary>
public class Monster2 : MonsterBase
{
    [Header("거리설정")]
    public float keepDistance = 7f; 

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

        //가까우면 도망
        if (dis < keepDistance)
        {
            agent.isStopped = false;
            agent.updateRotation = true;

            Vector3 dir = (transform.position - player.transform.position).normalized;
            Vector3 escapePoint = transform.position + dir * 2f;  // 뒤로 2m 이동
            agent.SetDestination(escapePoint);
        }
        //도망간후 다시 접근
        else if (dis > keepDistance)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(player.transform.position);
        }
        
        else
        {
            agent.isStopped = true;
            agent.updateRotation = false;

            state = Enums.MonsterState.Attack;
            timer = 0f;
        }
    }

    protected override void Attack()
    {
        if (player == null || md == null) return;

        agent.isStopped = true;
        agent.updateRotation = false;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        //거리 멀어지면 다시이동
        if (dis != keepDistance)
        {
            state = Enums.MonsterState.Move;
            agent.updateRotation = true;
            anim.SetTrigger("Move");
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer < md.attackSpeed) return;

        anim.SetTrigger("Shoot"); 

        timer = 0f;
    }

    // 원거리 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, keepDistance); // 유지거리

        Gizmos.color = Color.red;
        if (md != null) Gizmos.DrawWireSphere(transform.position, md.attackRange); //사거리
    }
}
