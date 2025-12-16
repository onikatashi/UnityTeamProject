using UnityEngine;

public class Enums
{
    // 아이템 등급
    public enum ItemRank
    {
        Common,
        Rare,
        Unique,
        Legendary
    }

    // 아이템 시너지
    // 임시 시너지 3개
    public enum ItemSynergy
    {
        None,
        Fire,
        Ice,
        Wind
    }

    // 아이템 추가 효과
    // 이건 정해진게 없음. 예시일 뿐임
    public enum ItemEffect
    {
        None,
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
        None,
        Normal,
        Elite,
        Shop,
        Rest,
        Forge,
        Boss
    }

    // 던전 테마
    public enum DungeonTheme
    {
        None,
        Desert,
        Grass,
        Lava,
        Snow
    }

    // 클래스(직업) 종류
    public enum ClassType
    {
        Warrior,
        Mage,
        Archer
    }

    // 몬스터 상태 종류
    // 텔레포트, 도망, 달리기(?), 패트롤 등 추가적으로 생각
    public enum MonsterState
    {
        Idle,
        Move,
        Attack,
        Die
    }
    public enum JumpPhase
    {
        None,
        Windup,
        Air,
        LandRecovery
    }

    // 인벤토리 모드
    public enum InventoryMode
    {
        None,               // 기본 상태
        Swap,               // 아이템 위치 교환 모드
        RankUp,             // 아이템 등급 상승 모드
        RanKUpWithSynergy   // 시너지 유지 아이템 등급 상승 모드
    }

    // 오브젝트 풀링 오브젝트 타입
    public enum PoolType
    {
        DescriptionSynergy,
        SynergyEffects,
        SynergyStatText,
        ArrowPool,
        MonsterProjectile,
        MonsterArcProjectile
    }
}
