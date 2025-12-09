using UnityEngine;

/// <summary>
/// 모든 스킬이 공통으로 가져야 하는 틀
/// 이름
/// 아이콘
/// 쿨타임
/// 레벨
/// 실행 메서드
/// 레벨업 메서드
/// </summary>
public abstract class SkillBase : ScriptableObject
{
    //스킬 이름
    public string skillName;

    //스킬 아이콘
    public Sprite icon;

    //스칼 쿨타임
    public float cooldown = 1f;

    //스킬 현재 레벨
    public int level = 1;

    //마지막으로 스킬을 사용한 시간 (Time.time 기준으로)
    //초기값을 빡 땡겨서 게임 시작 시 바로 사용 가능하게 만듦
    protected float lastUseTime = -999f;

    /// <summary>
    /// 스킬 실행 함수
    /// 반드시 있어야 해서 abstract로 선언
    /// </summary>
    /// <param name="player"></param>
    public abstract void Execute(Player player);
    
    /// <summary>
    /// 스킬 레벨업 함수
    /// 일단 levelUp은 제공해주고, 더 추가할거 있으면 자식이 추가하기
    /// </summary>
    public virtual void LevelUp()
    {
        level++;
    }

    /// <summary>
    /// 쿨타임 체크 함수
    /// 마지막 사용 시간 + 쿨타임 한 시간이면 사용 가능
    /// </summary>
    /// <returns></returns>
    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }
}
