using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public event System.Action OnAllEnemiesCleared;

    [System.Serializable]
    public class PhaseData
    {
        [Tooltip("다음 페이즈로 넘어가기까지의 대기 시간")]
        public float delayBeforeNextPhase = 3f;

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

    [Header("페이즈 데이터")]
    public List<PhaseData> phases = new List<PhaseData>();

    List<GameObject> aliveEnemies = new List<GameObject>();
    bool spawningFinished = false;


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

            // 페이즈 시작 즉시 스폰
            foreach (var spawn in phase.spawns)
            {
                SpawnEnemies(spawn);
            }

            // 다음 페이즈까지 delay
            yield return new WaitForSeconds(phase.delayBeforeNextPhase);
        }

        spawningFinished = true;

        // 페이즈는 끝났지만 아직 살아있는 적이 있을 수 있음 → 계속 체크
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
    }


    IEnumerator CheckClearState()
    {
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
