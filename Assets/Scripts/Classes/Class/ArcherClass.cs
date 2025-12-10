using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Class/Archer")]
public class ArcherClass : ClassBase
{    
    public override void BasicAttack(Player p, LayerMask monsterLayer)
    {
        float atk = p.finalStats.attackDamage;     // 플레이어 공격력
        float range = p.finalStats.attackRange;    // 플레이어 사정거리

        // 가장 가까운 몬스터 찾기
        Collider[] monsters = Physics.OverlapSphere(p.transform.position, range, monsterLayer);

        float minDist = Mathf.Infinity;                 // 최소 거리 초기화
        Collider nearest = null;                        // 가장 가까운 몬스터 저장

        foreach (Collider m in monsters)
        {
            float dist = (m.transform.position - p.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearest = m;
            }
        }

        if (nearest == null) return;                     // 적이 없으면 종료

        //화살 하나 꺼내오기
        ArcherProjectile arrow = p.GetArrow();

        //위치/ 회전 초기화
        arrow.transform.position = p.transform.position;
        arrow.transform.rotation = Quaternion.identity;

        //이 화살이 누구 소속인지 알려주기 ( 나중에 ReturnArrow 쓸 수 있게 해주는 코드 )
        arrow.Init(p);

        //타겟과 데미지 전달
        arrow.SetTarget(nearest.transform, atk);

    }
}
