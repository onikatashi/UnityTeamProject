using UnityEngine;

/// <summary>
/// 1. 플레이어 스탯 (체력, 마나, 공격력, 방어력 등)
/// 2. 플레이어 행동 (이동, 공격, 대쉬 등)
/// 3. 플레이어 애니메이션 제어
/// </summary>
public class Player : MonoBehaviour
{    // 아이템 + 클래스의 최종 스탯들을 합쳐야함.
    // 스탯들 다 정리해주고, 여기있는 이동속도를 PlayerMove에 넣어주기 / 공격력, 공격속도 등을 PlayerAttack에 넣어주기
    // Player는 Prefab으로 DungeonManager에서 만들어서 진행
    public static Player Instance;

    public ClassData classStat;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        classStat.cBaseStat.moveSpeed = 10f;
    }

    public Stats GetFinalStat()
    {
        Stats finalStat = InventoryManager.Instance.GetInventoryTotalStats() + classStat.cBaseStat;
        return finalStat;
    }
}
