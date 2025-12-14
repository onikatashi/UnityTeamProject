using UnityEngine;

/// <summary>
/// 원거리 몬스터 + 힐 (힐 우선)
/// 힐 범위 내 아군이 피해 상태면 공격 대신 힐
/// 힐할 아군이 없으면 원래처럼 투사체 공격
/// </summary>
public class Monster5 : MonsterBase
{
    [Header("Ranged Combat")]
    public float tolerance = 2f;

    float attackTimer = 0f;

    [Header("Heal (Aura)")]
    public float healRange = 6f;
    public float healPerSecond = 8f;
    public float healTickInterval = 0.25f;
    public bool includeSelf = false;
    public LayerMask monster;

    float healTimer = 0f;

    protected override void Idle()
    {
        agent.isStopped = true;

        if (player == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);
        if (dis <= detectRange)
        {
            state = Enums.MonsterState.Move;
        }
    }

    protected override void Move()
    {
        if (player == null || md == null) return;

        // 힐 범위 내 피해 아군이 있으면 공격(=힐/공격 판단) 상태로
        if (HasInjuredAllyInHealRange())
        {
            agent.isStopped = true;
            agent.updateRotation = false;
            state = Enums.MonsterState.Attack;

            healTimer = 0f;
            attackTimer = 0f;
            return;
        }

        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (dis > md.attackRange + tolerance)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // 사거리 적당히 들어오면 공격 상태로
            agent.isStopped = true;
            agent.updateRotation = false;

            state = Enums.MonsterState.Attack;
            attackTimer = 0f;
            healTimer = 0f;
        }
    }

    protected override void Attack()
    {
        if (player == null || md == null) return;

        agent.isStopped = true;
        agent.updateRotation = false;

        // 힐 우선 - 주변 아군이 피해 상태면 공격 안 하고 힐
        if (HasInjuredAllyInHealRange())
        {
            healTimer += Time.deltaTime;
            if (healTimer >= healTickInterval)
            {
                float healThisTick = healPerSecond * healTickInterval;
                HealAlliesInRange(healThisTick);
                healTimer = 0f;

                // anim.SetTrigger("Heal");
            }

            float disToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (disToPlayer > md.attackRange + tolerance)
            {
                state = Enums.MonsterState.Move;
                agent.updateRotation = true;
            }

            return; // 힐 중이면 공격 스킵
        }

        // 힐할 아군이 없으면 공격
        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (dis > md.attackRange + tolerance)
        {
            state = Enums.MonsterState.Move;
            agent.updateRotation = true;
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer < md.attackSpeed) return;

        
        MonsterProjectile mp = GetMonsterProjectile();

        Vector3 shootPos = firepoint != null ? firepoint.position : transform.position;
        mp.transform.position = shootPos;

        Vector3 targetPos = player.transform.position;
        Vector3 dir = (targetPos - shootPos).normalized;

        if (dir.sqrMagnitude > 0.0001f)
            mp.transform.rotation = Quaternion.LookRotation(dir);

        mp.Init(this, dir);

        // anim.SetTrigger("Shoot");

        attackTimer = 0f;
    }

    bool HasInjuredAllyInHealRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, healRange, monster);

        for (int i = 0; i < hits.Length; i++)
        {
            MonsterBase ally = hits[i].GetComponentInParent<MonsterBase>();
            if (ally == null) continue;

            if (!includeSelf && ally == this) continue;
            if (ally.currentHp <= 0f) continue;

            float maxHp = GetMaxHp(ally);
            if (maxHp <= 0f) continue;

            if (ally.currentHp < maxHp - 0.001f)
                return true;
        }

        return false;
    }

    void HealAlliesInRange(float healAmount)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, healRange, monster);

        for (int i = 0; i < hits.Length; i++)
        {
            MonsterBase ally = hits[i].GetComponentInParent<MonsterBase>();
            if (ally == null) continue;

            if (!includeSelf && ally == this) continue;
            if (ally.currentHp <= 0f) continue;

            float maxHp = GetMaxHp(ally);
            if (maxHp <= 0f) continue;

            if (ally.currentHp >= maxHp) continue;

            ally.currentHp = Mathf.Min(ally.currentHp + healAmount, maxHp);
        }
    }


    float GetMaxHp(MonsterBase m)
    {
        if (m.md != null)
            return m.md.maxHp;

        return 100f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);

        Gizmos.color = Color.red;
        if (md != null) Gizmos.DrawWireSphere(transform.position, md.attackRange);
    }
}
