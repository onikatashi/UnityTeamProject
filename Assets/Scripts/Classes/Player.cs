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

    public float currentHp;
    public float currentMp;

    ////스킬(버프) 곱연산용 변수
    //public float sRate;
    ////아이템(버프) 곱연산용 변수
    //public float iRate;

    public Stats finalStats;

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
        //sRate = 1f;
        //iRate = 1f;
        finalStats = GetFinalStat();
        currentHp = finalStats.maxHp;
        currentMp = finalStats.maxMp;
    }

    //void LookAtTheCamera()
    //{
    //    Transform target = Camera.main.transform;
    //    transform.rotation = transform.LookAt(target,target.tra);
    //}

    public Stats GetFinalStat()
    {
        return InventoryManager.Instance.GetInventoryTotalStats() + classStat.cBaseStat;     // 여기에 곱연산을 넣어주기?
    }

    public void TakeDamage(float value)
    {
        //몬스터 쪽에서 들고가야함
        currentHp -= value;

        // 애니메이션 넣을거면 넣고, 피격효과 넣을거면 여기 넣어줘야함.
    }
}
