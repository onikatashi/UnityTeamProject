using UnityEngine;
/// <summary>
/// 원거리 몬스터 패턴1
/// </summary>
public class Monster2 : MonsterBase
{
    
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

        if (dis < md.attackRange)
        {
            agent.isStopped = false;
            agent.updateRotation = true;

            Vector3 dir = (transform.position - player.transform.position).normalized;
            Vector3 escapePoint = transform.position + dir * 2f; 
            agent.SetDestination(escapePoint);
        }
        
        else if (dis > md.attackRange)
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

        //거리 멀어지면 다시 이동
        if (dis != md.attackRange)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, md.attackRange);

        Gizmos.color = Color.red;
        if (md != null) Gizmos.DrawWireSphere(transform.position, md.attackRange);
    }
}
