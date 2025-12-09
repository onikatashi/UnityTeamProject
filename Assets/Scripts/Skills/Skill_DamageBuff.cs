using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/DamageBuff")]
public class Skill_DamageBuff : SkillBase
{
    //레벨 1 기준 추가 공격력 수치
    public float baseDamage = 2f;

    //지속 시간
    public float duration = 5f;

    /// <summary>
    /// 스킬 실행 함수
    /// </summary>
    /// <param name="player"></param>
    public override void Execute(Player player)
    {
        //쿨타임이면 리턴
        if (!CanUse()) return;

        //마지막 사용 시간 갱신
        lastUseTime = Time.time;

        //코루틴 실행
        player.StartCoroutine(DoDamageBuff(player));
    }

    private IEnumerator DoDamageBuff(Player player)
    {
        //초기값 저장
        float originalDamage = player.finalStats.attackDamage;
        //버프 주기
        player.finalStats.attackDamage *= baseDamage;
        //지속시간
        yield return new WaitForSeconds(duration);
        //초기값으로 돌려주기
        player.finalStats.attackDamage = originalDamage;
    }

    public override void LevelUp()
    {
        base.LevelUp();

        //데미지 추가량 증가
        baseDamage *= 1.2f;
    }
}
