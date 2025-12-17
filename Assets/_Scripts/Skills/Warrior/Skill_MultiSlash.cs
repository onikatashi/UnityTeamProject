using UnityEngine;
using System.Collections;

/// <summary>
/// 0.4초 간격으로 전방향 공격을 3번 하는 스킬
/// </summary>
[CreateAssetMenu(menuName = "Skills/MultiSlash")]
public class Skill_MultiSlash : SkillBase
{
    
    public float baseDamage = 10f;          //레벨 1 기준 추가 데미지
    public float range = 4f;                //공격 범위 반경
    public float hitInterval = 0.4f;        //타격 사이 간격
    public int baseHitCount = 3;            //기본 타격 횟수 (1레벨 기준 3회)

    public LayerMask monsterLayer;          //몬스터 레이어 확인용

    /// <summary>
    /// 스킬 실행 함수
    /// SkillBase의 abstract Execute를 override 해서 실제 동작 구현
    /// </summary>
    /// <param name="player">스킬을 사용하는 플레이어</param>
    public override void Execute(Player player)
    {
        //쿨타임 안끝났으면 리턴
        if (!CanUse()) return;

        //마지막 사용 시간 갱신
        lastUseTime = Time.realtimeSinceStartup;

        //Coroutine은 MonoBehaviour에서만 실행 가능하므로 Player를 통해 코루틴 실행
        player.StartCoroutine(DoSlash(player));
    }

    private IEnumerator DoSlash(Player player)
    {
        //몇번 때릴지 결정 (레벨업하면 횟수 증가까지)
        int hitCount = baseHitCount + (level - 1);

        for (int i = 0; i < hitCount; i++)
        {
            //플레이어 위치 기준 원형 범위 안에 있는 모든 몬스터 찾아오기
            Collider[] monsters = Physics.OverlapSphere(
                player.transform.position,                  //플레이어기준
                range,                                      //범위안에
                monsterLayer);                              //몬스터레이어

            //데미지 계산 (플레이어 현재 공격력 + 스킬 데미지)
            float damage = player.finalStats.attackDamage + baseDamage * level;

            foreach(var c in monsters)
            {
                MonsterBase m = c.GetComponent<MonsterBase>();
                if( m!= null) m.TakeDamage(damage);
            }

            yield return new WaitForSeconds(hitInterval);
        }
    }

    public override void LevelUp()
    {
        //기본적으로 있는 것
        base.LevelUp();

        //추가로 자신의 데미지도 증가
        baseDamage *= 1.5f;
    }
}
