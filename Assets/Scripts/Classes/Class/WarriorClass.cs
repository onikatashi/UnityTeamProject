using UnityEngine;

[CreateAssetMenu(menuName = "Class/Warrior")]
public class WarriorClass : ClassBase
{
    public override void BasicAttack(Player p, LayerMask monster)
    {
        float atk = p.finalStats.attackDamage;          //Player 스탯 (공격력)
        float rng = p.finalStats.attackRange;           //Player 스탯 (사정거리)

        Collider[] monsters = Physics.OverlapSphere(p.transform.position, rng, monster);

        float minDist = Mathf.Infinity;
        Collider nearest = null;

        foreach (Collider m in monsters)
        {
            float dist = (m.transform.position - p.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearest = m;
            }
        }

        if (nearest == null) return;

        Vector3 dir = (nearest.transform.position - p.transform.position).normalized;
        float angle = Vector3.Angle(p.transform.forward, dir);

        if (angle <= 20f) // Warrior 기본 : 좁은 전방 공격
        {
            MonsterBase mob = nearest.GetComponent<MonsterBase>();
            if (mob != null) mob.TakeDamage(atk);
        }
    }
}
