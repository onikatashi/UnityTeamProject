using UnityEditor.ShaderGraph.Internal;
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
        currentHp = GetFinalStat().maxHp;
        currentMp = GetFinalStat().maxMp;
    }

    public Stats GetFinalStat()
    {
        Stats finalStat = InventoryManager.Instance.GetInventoryTotalStats() + classStat.cBaseStat;
        return finalStat;
    }

    public void TakeDamage(float value)
    {
        // 방어력(defense)이 있다면 데미지 감소 로직 추가 가능
        // float finalDamage = Mathf.Max(0, value - GetFinalStat().defense); 
        float finalDamage = value;

        // 몬스터 쪽에서 들고가야함 -> 플레이어가 데미지를 받는 로직은 플레이어 쪽에 있어야 합니다.
        currentHp -= finalDamage;

        Debug.Log("Player took " + finalDamage + " damage. Current HP: " + currentHp);

        // 애니메이션 넣을거면 넣고, 피격효과 넣을거면 여기 넣어줘야함.
        if (currentHp <= 0)
        {
            Die(); // 사망 처리 함수 (구현 필요)
        }
    }

    private void Die()
    {
        // 플레이어 사망 처리 (게임 오버, 재시작 등)
        Debug.Log("Player has died.");
        // Time.timeScale = 0; // 예시
    }
}
