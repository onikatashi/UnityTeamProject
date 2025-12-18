using UnityEngine;

/// <summary>
/// 레벨
/// 쿨타임
/// 스킬 사용
/// </summary>
[System.Serializable]
public class SkillRuntime
{
    public SkillBase skillBaseData;         //스킬 베이스 데이터
    public int currentLevel;                //스킬 현재 레벨

    private float lastUseTime;              //쿨타임용 마지막 실행 시간

    public SkillRuntime(SkillBase baseData)
    {
        skillBaseData = baseData;
        currentLevel = 1;
        lastUseTime = -999f;
    }

    public bool CanUse()
    {
        //Time.time 을 사용하면 레벨업 시 게임 멈추기 가능
        return Time.time >= lastUseTime + skillBaseData.cooldown;
    }

    /// <summary>
    /// 스킬 사용
    /// </summary>
    /// <param name="player"></param>
    public void Use(Player player)
    {
        if (!CanUse()) return;              //쿨타임이거나, 상태이상일 때 사용 불가

        lastUseTime = Time.time;            //쿨타임 적용
        skillBaseData.Execute(player, currentLevel);
    }

    /// <summary>
    /// 스킬 레벨업
    /// </summary>
    public void LevelUp()
    {
        currentLevel++;
    }

    /// <summary>
    /// 스킬 쿨타임 적용
    /// </summary>
    /// <returns></returns>
    public float GetCooldownRemaining()
    {
        float endTime = lastUseTime + skillBaseData.cooldown;
        return Mathf.Max(0f, endTime - Time.time);
    }
}
