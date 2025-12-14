using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

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

    //직업 데이터 넣는곳, 처음은 Warrior고정, 직업 변경시 바꿔줘야함.
    public ClassData classStat;

    public float currentHp;
    public float currentMp;

    ////스킬(버프) 곱연산용 변수
    //public float sRate;
    ////아이템(버프) 곱연산용 변수
    //public float iRate;

    public Stats finalStats;
    bool isStunned = false;
    public bool IsStunned => isStunned;
    bool isInputReversed = false;
    public bool IsInputReversed => isInputReversed;


    //아처 오브젝트 풀 큐 만들기
    //ScriptableObject(Job_Archer)는 “씬에 있는 풀”을 직접 관리하면 안 됨
    PoolManager poolManager;                    //캐싱용
    public ArcherProjectile arrowPrefab;        //화살 프리팹
    public int arrowPoolSize = 5;               //풀 화살 갯수 저장

    // 플레이어 Sprite 항상 카메라 바라보기
    public CinemachineCamera pCam;
    Transform pSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        pSprite = transform.Find("Sprite");

        poolManager = PoolManager.Instance;
    }
    private void Start()
    {
        finalStats = GetFinalStat();
        currentHp = finalStats.maxHp;
        currentMp = finalStats.maxMp;
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowPrefab이 비어있음!");
            return; // 또는 예외 처리하고 진행 중단
        }
        poolManager.CreatePool<ArcherProjectile>(Enums.PoolType.ArrowPool, arrowPrefab, arrowPoolSize, transform);
    }

    private void Update()
    {
        LookAtPCam();
    }

    /// <summary>
    /// 최종 데미지 스탯 갱신해주기
    /// </summary>
    /// <returns></returns>
    public Stats GetFinalStat()
    {
        return InventoryManager.Instance.GetInventoryTotalStats() + classStat.cBaseStat;     
    }

    /// <summary>
    /// 플레이어가 데미지 입었을 때 호출될 함수
    /// </summary>
    /// <param name="value"></param>
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

    /// <summary>
    /// 체력 회복 함수 (스킬이나 상호작용 등에 쓰일 예정)
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        //Heal 수치만큼 현재 체력 회복
        currentHp += amount;
        //최대체력을 넘어가면, 
        if (currentHp >= finalStats.maxHp)
        {
            currentHp = finalStats.maxHp;
        }
    }

    /// <summary>
    /// 플레이어 죽을 시 실행될 함수
    /// </summary>
    private void Die()
    {
        // 플레이어 사망 처리 (게임 오버, 재시작 등)
        Debug.Log("Player has died.");
        // Time.timeScale = 0; // 예시
    }

    /// <summary>
    /// 화살 하나 꺼내오기
    /// </summary>
    /// <returns></returns>
    public ArcherProjectile GetArrow()
    {
        return poolManager.Get<ArcherProjectile>(Enums.PoolType.ArrowPool);
    }

    /// <summary>
    /// 화살 집어넣기 (Projectile에서 이 함수 호출)
    /// </summary>
    /// <param name="arrow"></param>
    public void ReturnArrow(ArcherProjectile arrow)
    {
        poolManager.Return<ArcherProjectile>(Enums.PoolType.ArrowPool, arrow);
    }

    /// <summary>
    /// 캐릭터 Sprite가 항상 카메라를 바라보게 하기
    /// 맵 상에 있는 Sprite들은 어지간하면 다 이걸 적용시켜줘야 함.
    /// </summary>
    void LookAtPCam()
    {
        pSprite.transform.LookAt(pCam.transform.position);
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StopCoroutine(nameof(CoStun));
        StartCoroutine(CoStun(duration));
    }

    IEnumerator CoStun(float d)
    {
        isStunned = true;
        
        yield return new WaitForSeconds(d);

        isStunned = false;
    }
    public void ReverseInput(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StopCoroutine(nameof(CoReverse));
        StartCoroutine(CoReverse(duration));
    }
    IEnumerator CoReverse(float d)
    {
        isInputReversed = true;
        yield return new WaitForSeconds(d);
        isInputReversed = false;
    }
}
