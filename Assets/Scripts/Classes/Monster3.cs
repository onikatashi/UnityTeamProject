using UnityEngine;
/// <summary>
/// 폭8 몬스터 패턴1
/// </summary>
public class Monster3 : MonsterBase
{
    [Header("폭8범위")]
    public float explosionRanege = 3f;

    public LayerMask Player;

    protected override void Idle()
    {
        agent.isStopped = true;

        if (player == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);
        if (dis <= detectRange)
        {
            state = Enums.MonsterState.Move;
            //anim.SetTrigger("Move");
        }
    }

    protected override void Move()
    {
        if (player == null || md == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (dis > md.attackRange)
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
            //anim.SetTrigger("Attack");
        }
    }

    protected override void Attack()
    {
        if (player == null || md == null) return;

        agent.isStopped = true;
        agent.updateRotation = false;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (dis > md.attackRange * 1.5f)
        {
            state = Enums.MonsterState.Move;
            agent.updateRotation = true;
            //anim.SetTrigger("Move");
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer < md.attackSpeed) return;

        Debug.Log("자폭");
        Explode();
    }

    private void Explode()
    {
        //anim.SetTrigger("Explode");

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRanege, Player); // OverlapSphere -> 원형범위내의 오브젝트를 찾는 함수

        foreach (var hit in hits)
        {
            if (player != null)
            {
                var hp = hit.GetComponent<Player>();
                if (hp != null)
                {
                    hp.TakeDamage(md.attackDamage);
                }
            }
        }

        state = Enums.MonsterState.Die;
        Die();
    }

        
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRanege);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, md.attackRange);
    }
}
