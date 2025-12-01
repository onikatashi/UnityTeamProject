using UnityEngine;

public class Enums
{
    // 아이템 등급
    public enum ItemRank
    {
        Normal,
        Rare,
        Unique,
        Legendery
    }

    // 아이템 시너지
    // 임시 시너지 3개
    public enum ItemSynergy
    {
        Fire,
        Ice,
        Wind
    }

    // 아이템 추가 효과
    // 이건 정해진게 없음. 예시일 뿐임
    public enum ItemEffect
    {
        AutoAttack,
        AutoBuff,
        LifeSteal
    }

    // 몬스터 등급(종류)
    public enum MonsterRank
    {
        Normal,
        Elite,
        Boss
    }

    // 방(노드) 종류
    public enum RoomType
    {
        Normal,
        Elite,
        Shop,
        Rest,
        Forge,
        Boss
    }

    // 클래스(직업) 종류
    public enum ClassType
    {
        Warrior,
        Mage,
        Archer
    }
}
