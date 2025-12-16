using UnityEngine;

public class MonsterLaser : MonoBehaviour
{
    [Header("Hit")]
    public float width = 0.6f;
    public float length = 18f;
    public LayerMask playerMask;

    bool firing = false;

    public void BeginWindup(float windup)
    {
        // 여기서 레이저 차징 연출을 켜고,
        // windup 시간 후 BeginFire()가 호출되도록 연결하면 됨
    }

    public void BeginFire() => firing = true;
    public void EndFire() => firing = false;

    public void UpdateAim(Vector3 origin, Vector3 forward)
    {
        // 레이저 시작 위치를 약간 위로 보정
        transform.position = origin + Vector3.up * 1.2f;

        // 유효한 방향 벡터가 있을 때만 회전 적용
        if (forward.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(forward);
    }

    public void TryHit(Player player, float damagePerTick)
    {
        if (!firing || player == null) return;

        // 레이저를 박스 오버랩(OverlapBox) 방식으로 판정
        // 폭과 길이를 가진 레이저 충돌 영역을 사용
        Vector3 origin = transform.position;
        Vector3 halfExtents = new Vector3(width * 0.5f, 1.0f, length * 0.5f);
        Vector3 center = origin + transform.forward * (length * 0.5f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation, playerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                // 레이저 틱 데미지 적용
                p.TakeDamage(damagePerTick);
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // 레이저 판정 범위 시각화
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(0, 0, length * 0.5f), new Vector3(width, 2f, length)
        );
    }
}
