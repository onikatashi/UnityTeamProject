using System.Collections;
using UnityEngine;

public class Monster10 : MonsterBase
{
    float timer = 0f;
    public float tolerance = 2f;

    [Header("Mark Bomb")]
    public GroundTelegraph telegraphPrefab;
    public float markDelay = 1.2f;        // 표시 후 폭발까지
    public float explodeRadius = 3.5f;
    public float stunDuration = 1.0f;
    public LayerMask groundMask;          // (선택) 바닥용

    [Header("Damage")]
    public float damageMultiplier = 1f;   // md.attackDamage * multiplier

    bool casting = false;

    protected override void Idle()
    {
        agent.isStopped = true;
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) <= detectRange)
            state = Enums.MonsterState.Move;
    }

    protected override void Move()
    {
        if (player == null || md == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (dis > md.attackRange + tolerance)
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
        if (player == null || md == null || casting) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);
        if (dis > md.attackRange + tolerance)
        {
            state = Enums.MonsterState.Move;
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer < md.attackSpeed) return;

        // 발사 순간 플레이어 위치 고정
        Vector3 targetPos = player.transform.position;

        StartCoroutine(CoMarkExplode(targetPos));

        timer = 0f;
    }

    IEnumerator CoMarkExplode(Vector3 pos)
    {
        casting = true;

        // 차징생성
        GroundTelegraph tg = Instantiate(telegraphPrefab, pos, Quaternion.Euler(90f, 0f, 0f));
        tg.Setup(explodeRadius, markDelay);
        tg.StartCharge();

        // 대기
        yield return new WaitForSeconds(markDelay);

        // 폭발 판정
        Collider[] hits = Physics.OverlapSphere(pos, explodeRadius);
        int playerLayer = LayerMask.NameToLayer("Player");

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer != playerLayer) continue;

            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                float dmg = md.attackDamage * damageMultiplier;
                p.TakeDamage(dmg);

                // 기절
                p.Stun(stunDuration);
            }
        }

        tg.StopAndHide();
        Destroy(tg.gameObject);

        casting = false;
    }
}
