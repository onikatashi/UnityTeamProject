using UnityEngine;

public class MonsterArcProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float gravity = 25f;
    public float destroyDistance = 50f;
    public LayerMask groundMask;

    [Header("Explosion")]
    public float explodeRadius = 4.5f;    // 범위
    public float stunDuration = 2f;     // 기절 시간
    
    Vector3 velocity;
    Vector3 startPos;

    MonsterBase owner;
    PoolManager pool;
    int playerLayer;

    void Awake()
    {
        pool = PoolManager.Instance;
        playerLayer = LayerMask.NameToLayer("Player");
    }

    /// <param name="arcHeight">포물선 최고점 높이(클수록 더 높이 뜸)</param>
    public void Init(MonsterBase owner, Vector3 from, Vector3 to, float arcHeight)
    {
        this.owner = owner;

        transform.position = from;
        startPos = from;

        // 초기 속도 계산
        velocity = CalculateArcVelocity(from, to, arcHeight);

        gameObject.SetActive(true);
    }

    void Update()
    {
        // 중력
        velocity.y -= gravity * Time.deltaTime;

        // 이동
        transform.position += velocity * Time.deltaTime;

        // 날아가는 방향으로 회전
        if (velocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(velocity.normalized);

        // 너무 멀어지면 회수
        if ((transform.position - startPos).sqrMagnitude > destroyDistance * destroyDistance)
        {
            ReturnToPool();
        }
    }

    // 지면 충돌 감지 - Collider가 Trigger여도 바닥 체크하려면 Raycast가 안전함
    void LateUpdate()
    {
        // 아래로 짧게 레이 쏴서 지면 닿으면 폭발
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out var hit, 0.25f, groundMask))
        {
            Explode();
        }
    }

    void Explode()
    {
        // 이미 비활성화 되었거나, owner가 없는 경우 방어
        if (!gameObject.activeSelf) return;

        // 폭발 범위 내 플레이어 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, explodeRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer != playerLayer) continue;

            Player p = hits[i].GetComponent<Player>();
            if (p == null || owner == null || owner.md == null) continue;

            float dmg = owner.md.attackDamage;

            BuffReceiver buff = owner.GetComponent<BuffReceiver>();
            if (buff != null) dmg *= buff.AttackMultiplier;

            p.TakeDamage(dmg);
            p.Stun(stunDuration);
        }

        ReturnToPool();
    }

    Vector3 CalculateArcVelocity(Vector3 from, Vector3 to, float arcHeight)
    {
        // 포물선 비행 시간을 높이로 대충 결정
        // 높이를 크게 주면 time이 늘고, 수평 속도가 줄어듦
        float timeUp = Mathf.Sqrt(2f * arcHeight / gravity);
        float timeDown = timeUp;
        float totalTime = Mathf.Max(0.15f, timeUp + timeDown);

        Vector3 delta = to - from;
        Vector3 deltaXZ = new Vector3(delta.x, 0f, delta.z);

        float vxz = deltaXZ.magnitude / totalTime;
        Vector3 dirXZ = (deltaXZ.sqrMagnitude > 0.0001f) ? deltaXZ.normalized : Vector3.zero;

        float vy = gravity * timeUp; // 최고점까지 올라가기 위한 초기 y속도

        return dirXZ * vxz + Vector3.up * vy;
    }

    void ReturnToPool()
    {
        pool.Return(Enums.PoolType.MonsterArcProjectile, this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
