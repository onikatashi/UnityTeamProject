using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // LINQ 사용을 위해 추가

public class SpawnManager : MonoBehaviour
{
    public event System.Action OnAllEnemiesCleared;

    [System.Serializable]
    public class PhaseData
    {
        // 이전 PhaseData에 있던 delayBeforeNextPhase는 제거되었습니다.
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
    [Tooltip("다음 페이즈로 넘어가기까지의 인터벌 시간 (이 시간 내에 모든 적 제거 시 즉시 다음 페이즈)")]
    public float IntervalTime = 3f; // Delay Befor Next Phase → Interval Time (전역 설정)

    [Header("페이즈 데이터")]
    public List<PhaseData> phases = new List<PhaseData>();

    List<GameObject> aliveEnemies = new List<GameObject>();
    bool spawningFinished = false;

    // 현재 페이즈 진행 여부를 체크하는 변수 추가
    bool isPhaseActive = false;


    /// <summary>
    /// DungeonManager 에서 직접 호출
    /// </summary>
    public void StartSpawning()
    {
        StartCoroutine(SpawnFlow());
    }


    IEnumerator SpawnFlow()
    {
        for (int i = 0; i < phases.Count; i++)
        {
            var phase = phases[i];
            Debug.Log($"== {i + 1} 페이즈 시작 ==");
            isPhaseActive = true; // 페이즈 시작

            // 페이즈 시작 즉시 스폰
            foreach (var spawn in phase.spawns)
            {
                SpawnEnemies(spawn);
            }

            // 다음 페이즈 진입 조건:
            // 1. IntervalTime 동안 대기 (시간 초과)
            // 2. IntervalTime 내에 모든 적을 제거하면 즉시 다음 페이즈로 이동 (WaitUntil 조건)

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

            isPhaseActive = false; // 페이즈 대기 종료

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


    void SpawnEnemies(SpawnInfo info)
    {
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


    // Enemy 가 사망 시 호출될 함수
    public void NotifyEnemyDeath(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);

        // **중요:** 인터벌 타임 내에 다음 페이즈로 즉시 넘어가도록 하기 위해
        // CheckClearState 대신 SpawnFlow 코루틴 내에서 상태를 확인합니다.
    }


    IEnumerator CheckClearState()
    {
        // 이 코루틴은 모든 페이즈가 끝난 후에만 호출됩니다.
        while (true)
        {
            if (spawningFinished && aliveEnemies.Count == 0)
            {
                Debug.Log("모든 적 제거 → 던전 클리어");
                OnAllEnemiesCleared?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}


// 적이 죽었을 때 SpawnManager에 알리기 위한 간단한 바인더
public class EnemyLifeBinder : MonoBehaviour
{
    public SpawnManager manager;

    private void OnDestroy()
    {
        if (manager != null)
            manager.NotifyEnemyDeath(gameObject);
    }
}