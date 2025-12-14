using UnityEngine;


public class Monster8 : MonsterBase
{
    enum JumpPhase { None, Windup, Air, LandRecovery }

    [Header("Jump Attack")]
    public float jumpMinDistance = 2.5f;   // 너무 가까우면 점프 안 함
    public float jumpMaxDistance = 6.0f;   // 이 거리 안이면 점프 시도
    public float windupTime = 0.25f;       // 점프 전 준비시간
    public float airTime = 0.45f;          // 공중 이동 시간
    public float landingRecovery = 0.25f;  // 착지 후 딜레이
    public float landingHitRadius = 1.8f;  // 착지 판정 반경
    public float jumpArcHeight = 1.5f;     // 점프 포물선 높이

    float timer = 0f;
    JumpPhase phase = JumpPhase.None;

    Vector3 jumpStartPos;
    Vector3 jumpTargetPos;   // 점프 시작시 목표 고정
    float phaseStartTime;

    int playerLayer;

    protected override void Awake()
    {
        base.Awake();
        playerLayer = LayerMask.NameToLayer("Player");
    }

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

        float dis = Vector3.Distance(transform.position, player.transform.position);

        // 점프 공격 조건: 일정 거리 안
        if (dis <= jumpMaxDistance && dis >= jumpMinDistance)
        {
            agent.isStopped = true;
            agent.updateRotation = false;

            state = Enums.MonsterState.Attack;
            phase = JumpPhase.Windup;
            phaseStartTime = Time.time;

            jumpStartPos = transform.position;
            jumpTargetPos = player.transform.position;

            return;
        }

        // 점프 조건 아니면 기존 근접 추적
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
        }
    }

    protected override void Attack()
    {
        if (player == null || md == null) return;

        // Attack 상태는 점프 공격 전용
        switch (phase)
        {
            case JumpPhase.Windup:
                HandleWindup();
                break;
            case JumpPhase.Air:
                HandleAir();
                break;
            case JumpPhase.LandRecovery:
                HandleLandRecovery();
                break;
            default:
                // 안전장치
                phase = JumpPhase.None;
                state = Enums.MonsterState.Move;
                break;
        }
    }

    void HandleWindup()
    {
        agent.isStopped = true;
        agent.enabled = false;

        // anim.SetTrigger("Jump");

        if (Time.time - phaseStartTime >= windupTime)
        {
            phase = JumpPhase.Air;
            phaseStartTime = Time.time;
        }
    }

    void HandleAir()
    {
        float t = Mathf.Clamp01((Time.time - phaseStartTime) / airTime);

        // 목표는 jumpTargetPos 고정. 공중에서 player 위치 안 봄.
        Vector3 flatPos = Vector3.Lerp(jumpStartPos, jumpTargetPos, t);

        // 포물선(위로 솟았다가 내려오게)
        float arc = Mathf.Sin(t * Mathf.PI) * jumpArcHeight;
        Vector3 pos = new Vector3(flatPos.x, flatPos.y + arc, flatPos.z);

        transform.position = pos;

        if (t >= 1f)
        {
            // 착지
            DoLandingHitOnce();

            phase = JumpPhase.LandRecovery;
            phaseStartTime = Time.time;

        }
    }

    void HandleLandRecovery()
    {
        if (Time.time - phaseStartTime >= landingRecovery)
        {
            // NavMesh 복구
            agent.enabled = true;
            agent.isStopped = false;
            agent.updateRotation = true;

            phase = JumpPhase.None;
            state = Enums.MonsterState.Move;
        }
    }

    void DoLandingHitOnce()
    {
        // 착지 지점 기준으로 플레이어가 반경 안이면 데미지
        Collider[] hits = Physics.OverlapSphere(transform.position, landingHitRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer != playerLayer) continue;

            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(md.attackDamage);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        if (md != null) Gizmos.DrawWireSphere(transform.position, md.attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jumpMaxDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, landingHitRadius);
    }

}
