using System.Collections;
using UnityEngine;

public class Monster11 : MonsterBase
{
    float timer = 0f;

    [Header("Pull")]
    public float pullRange = 8f;
    public float pullCooldown = 4f;
    public float pullDuration = 0.35f;
    public float pullStrength = 12f; // 클수록 빨리 끌림

    float lastPull = -999f;
    bool pulling = false;

    protected override void Idle()
    {
        agent.isStopped = true;
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) <= detectRange)
        {
            state = Enums.MonsterState.Move;
        }
    }

    protected override void Move()
    {
        if (player == null || md == null) return;

        float dis = Vector3.Distance(transform.position, player.transform.position);

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.SetDestination(player.transform.position);

        if (dis <= pullRange && Time.time >= lastPull + pullCooldown)
        {
            state = Enums.MonsterState.Attack;
            timer = 0f;
        }
    }

    protected override void Attack()
    {
        if (player == null || pulling) 
        { 
            state = Enums.MonsterState.Move; return; 
        }

        // 풀 스킬 1회 실행
        lastPull = Time.time;
        StartCoroutine(CoPullPlayer());

        state = Enums.MonsterState.Move;
    }

    IEnumerator CoPullPlayer()
    {
        pulling = true;

        // anim.SetTrigger("Pull");

        Rigidbody rb = player.GetComponent<Rigidbody>();
        Transform pt = player.transform;

        float t = 0f;
        while (t < pullDuration)
        {
            t += Time.deltaTime;

            Vector3 dir = (transform.position - pt.position);
            dir.y = 0f;
            dir = dir.normalized;

            if (rb != null)
            {
                // Rigidbody면 물리적으로 끌기(속도 방식)
                Vector3 v = dir * pullStrength;
                rb.linearVelocity = new Vector3(v.x, rb.linearVelocity.y, v.z);
            }
            else
            {
                // Rigidbody 없으면 위치 보정(컨트롤러랑 충돌할 수 있음)
                pt.position += dir * pullStrength * Time.deltaTime;
            }

            yield return null;
        }

        // rb 속도 정리
        if (rb != null) rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

        pulling = false;
    }
}
