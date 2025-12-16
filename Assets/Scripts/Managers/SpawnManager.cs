using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // LINQ 사용을 위해 추가
using static Enums; // RoomType Enum 접근을 위해 필요. DungeonManager.cs에 이미 'using static Enums;'가 있으므로 가정합니다.
using Random = UnityEngine.Random; // UnityEngine.Random 명시

public class SpawnManager : MonoBehaviour
{
    public event System.Action OnAllEnemiesCleared;

    [System.Serializable]
    public class PhaseData
    {
        [Tooltip("이 페이즈에서 소환할 적들")]
        public List<SpawnInfo> spawns = new List<SpawnInfo>();
    }

    [System.Serializable]
    public class SpawnInfo
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;
        public int count = 3;
        public float spawnOffset = 1.0f;
    }

    [Header("전역 설정")]
    [Tooltip("다음 페이즈로 넘어가기까지의 인터벌 시간")]
    public float IntervalTime = 3f;

    // -----------------------------------------------------
    [Header("룸 타입별 페이즈 데이터")]
    [Tooltip("Normal 방에서 사용할 페이즈 설정")]
    public List<PhaseData> normalPhases = new List<PhaseData>();

    [Tooltip("Elite 방에서 사용할 페이즈 설정")]
    public List<PhaseData> elitePhases = new List<PhaseData>();

    [Tooltip("Boss 방에서 사용할 페이즈 설정")]
    public List<PhaseData> bossPhases = new List<PhaseData>();
    // -----------------------------------------------------

    // -----------------------------------------------------
    [Header("방 클리어 보상 설정 (DungeonManager에서 이동)")]

    [Tooltip("모든 리워드 슬롯에 사용될 프리팹")]
    public GameObject rewardPrefab; // Reward 프리팹 하나만 사용

    [Tooltip("리워드가 스폰될 첫 번째 위치")]
    public Transform reward1SpawnPoint;
    [Tooltip("리워드가 스폰될 두 번째 위치")]
    public Transform reward2SpawnPoint;
    [Tooltip("리워드가 스폰될 세 번째 위치")]
    public Transform reward3SpawnPoint;

    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    // -----------------------------------------------------


    List<GameObject> aliveEnemies = new List<GameObject>();
    bool spawningFinished = false;
    bool isPhaseActive = false;


    /// <summary>
    /// DungeonManager 에서 현재 방 타입과 함께 호출됩니다.
    /// </summary>
    public void StartSpawning(RoomType roomType)
    {
        // DungeonManager에서 구독자가 설정될 시간을 주기 위해 지연 시작
        StartCoroutine(StartSpawningFlow(roomType));
    }

    // 코루틴 시작을 위한 래퍼 함수
    IEnumerator StartSpawningFlow(RoomType roomType)
    {
        // DungeonManager에서 이벤트 구독을 완료할 때까지 대기 (선택적)
        yield return null;

        // 실제 스폰 로직 시작
        StartCoroutine(SpawnFlow(roomType));
    }


    IEnumerator SpawnFlow(RoomType currentRoomType)
    {
        // 1. 현재 룸 타입에 맞는 페이즈 리스트 선택
        List<PhaseData> currentPhases = GetPhasesForRoomType(currentRoomType);

        if (currentPhases == null || currentPhases.Count == 0)
        {
            Debug.LogWarning($"{currentRoomType} 타입에 설정된 페이즈 데이터가 없습니다. 스폰을 건너뜁니다.");
            spawningFinished = true;
            StartCoroutine(CheckClearState()); // 페이즈가 없더라도 클리어 상태 체크는 시작
            yield break;
        }

        for (int i = 0; i < currentPhases.Count; i++)
        {
            var phase = currentPhases[i];
            Debug.Log($"== {currentRoomType} - {i + 1} 페이즈 시작 ==");
            isPhaseActive = true;

            // 페이즈 시작 즉시 스폰
            foreach (var spawn in phase.spawns)
            {
                SpawnEnemies(spawn);
            }

            float timer = 0f;

            // IntervalTime 동안 다음 두 조건을 체크하며 대기합니다.
            while (timer < IntervalTime && isPhaseActive)
            {
                // 모든 적이 제거되었다면 (aliveEnemies.Count == 0) 즉시 루프 탈출
                if (aliveEnemies.Count == 0)
                {
                    Debug.Log($"모든 적 제거! {IntervalTime - timer:F2}초 남았지만 즉시 다음 페이즈로 이동");
                    break;
                }

                timer += Time.deltaTime;
                yield return null; // 1프레임 대기
            }

            isPhaseActive = false;

            // 만약 인터벌 타임이 초과되었는데도 적이 남아있다면 경고 로그 출력
            if (aliveEnemies.Count > 0 && timer >= IntervalTime)
            {
                Debug.Log($"인터벌 시간 초과 ({IntervalTime}초). 다음 페이즈로 강제 진입 (남은 적: {aliveEnemies.Count}마리)");
            }
        }

        spawningFinished = true;

        // 모든 페이즈가 끝났으므로 최종 클리어 상태 체크를 시작
        StartCoroutine(CheckClearState());
    }


    /// <summary>
    /// 현재 룸 타입에 맞는 페이즈 리스트를 반환합니다.
    /// </summary>
    private List<PhaseData> GetPhasesForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Normal:
                return normalPhases;
            case RoomType.Elite:
                return elitePhases;
            case RoomType.Boss:
                return bossPhases;
            default:
                Debug.LogWarning($"RoomType {roomType}에는 스폰 로직이 정의되어 있지 않습니다.");
                return null;
        }
    }


    void SpawnEnemies(SpawnInfo info)
    {
        // (기존 SpawnEnemies 함수 내용은 동일)
        for (int i = 0; i < info.count; i++)
        {
            float offsetX = Random.Range(-info.spawnOffset, info.spawnOffset);
            float offsetZ = Random.Range(-info.spawnOffset, info.spawnOffset);

            Vector3 pos = info.spawnPoint.position + new Vector3(offsetX, 0f, offsetZ);

            GameObject enemy = Instantiate(info.enemyPrefab, pos, info.spawnPoint.rotation);

            aliveEnemies.Add(enemy);

            // 적이 죽었을 때 리스트에서 제거하도록 EnemyHP 같은 스크립트에서 호출 필요
            EnemyLifeBinder binder = enemy.AddComponent<EnemyLifeBinder>();
            binder.manager = this;
        }

        Debug.Log($"{info.spawnPoint.name}에서 {info.count}마리 소환");
    }


    // Enemy 가 사망 시 호출될 함수 (기존과 동일)
    public void NotifyEnemyDeath(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }


    IEnumerator CheckClearState()
    {
        // (기존 CheckClearState 코루틴 내용은 동일)
        while (true)
        {
            if (spawningFinished && aliveEnemies.Count == 0)
            {
                Debug.Log("모든 적 제거 → 던전 클리어");
                OnAllEnemiesCleared?.Invoke(); // 방 클리어 이벤트 발생
                HandleRoomCleared(); // 리워드/포탈 생성 로직 호출
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 방 클리어 시 리워드 및 포탈 생성 로직
    /// </summary>
    private void HandleRoomCleared()
    {
        Debug.Log("== 던전 방 클리어 (리워드/포탈 생성) ==");

        SpawnAllRewards();
        SpawnPortal();
    }

    /// <summary>
    /// 설정된 3개의 리워드 슬롯을 확인하고 각각 스폰합니다. (rewardPrefab 하나 사용)
    /// </summary>
    private void SpawnAllRewards()
    {
        int rewardsSpawned = 0;

        // 하나의 rewardPrefab과 세 개의 다른 스폰 포인트를 사용
        if (SpawnSingleReward(rewardPrefab, reward1SpawnPoint, 1))
            rewardsSpawned++;

        if (SpawnSingleReward(rewardPrefab, reward2SpawnPoint, 2))
            rewardsSpawned++;

        if (SpawnSingleReward(rewardPrefab, reward3SpawnPoint, 3))
            rewardsSpawned++;

        Debug.Log($"총 {rewardsSpawned}개의 리워드가 생성되었습니다.");
    }

    /// <summary>
    /// 개별 리워드를 스폰합니다.
    /// </summary>
    private bool SpawnSingleReward(GameObject prefab, Transform spawnPoint, int slotIndex)
    {
        // 프리팹이 설정되지 않았거나 스폰 위치가 설정되지 않은 경우 처리
        if (prefab == null)
        {
            if (spawnPoint != null)
                Debug.LogWarning($"리워드 {slotIndex}의 프리팹이 설정되지 않았습니다. 스폰을 건너뜁니다.");
            return false;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning($"리워드 {slotIndex}의 스폰 위치가 설정되지 않았습니다. 스폰을 건너뜁니다.");
            return false;
        }

        // 스폰
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"리워드 {slotIndex} 생성 완료: {prefab.name} @ {spawnPoint.name}");
        return true;
    }


    private void SpawnPortal()
    {
        if (portalPrefab != null && portalSpawnPoint != null)
        {
            Instantiate(portalPrefab, portalSpawnPoint.position, portalSpawnPoint.rotation);
            Debug.Log("포탈 생성 완료");
        }
    }
}

// 적이 죽었을 때 SpawnManager에 알리기 위한 간단한 바인더 (기존과 동일)
public class EnemyLifeBinder : MonoBehaviour
{
    public SpawnManager manager;

    private void OnDestroy()
    {
        if (manager != null)
            manager.NotifyEnemyDeath(gameObject);
    }
}