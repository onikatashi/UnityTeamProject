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
        // 여기서 라인렌더러 켜고 색 바꾸고 등 연출 가능
    }

    public void BeginFire() => firing = true;
    public void EndFire() => firing = false;

    public void UpdateAim(Vector3 origin, Vector3 forward)
    {
        transform.position = origin + Vector3.up * 1.2f; // 높이 보정
        if (forward.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(forward);
    }

    public void TryHit(Player player, float damagePerTick)
    {
        if (!firing || player == null) return;

        // 레이저를 “박스캐스트”로 판정 (두께 있는 레이저 느낌)
        Vector3 origin = transform.position;
        Vector3 halfExtents = new Vector3(width * 0.5f, 1.0f, length * 0.5f);
        Vector3 center = origin + transform.forward * (length * 0.5f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation, playerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Player p = hits[i].GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(Player.Instance != null ? 1f : 1f);
                return;
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(0, 0, length * 0.5f), new Vector3(width, 2f, length));
    }
}
