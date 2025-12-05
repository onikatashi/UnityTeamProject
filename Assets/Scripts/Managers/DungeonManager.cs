using UnityEngine;
using System.Collections;

public class DungeonManager : MonoBehaviour
{
    // 소환할 적 프리팹 (Inspector에서 연결)
    public GameObject enemyPrefab;

    // 적 소환 위치 (Inspector에서 3개의 빈 오브젝트를 연결)
    public Transform[] spawnPoints = new Transform[3];

    // 페이즈 간 대기 시간
    public float timeBetweenPhases = 5.0f;

    // 소환 시 적용할 무작위 오프셋의 최대 범위
    public float maxSpawnOffset = 1.0f;

    void Start()
    {
        // 필수 요소들이 연결되었는지 확인합니다.
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab이 설정되지 않았습니다. DungeonManager.cs");
            return;
        }

        if (spawnPoints.Length < 3 || spawnPoints[0] == null || spawnPoints[1] == null || spawnPoints[2] == null)
        {
            Debug.LogError("EnemySpawnPoint가 3개 모두 올바르게 설정되지 않았습니다. DungeonManager.cs");
            return;
        }

        // 던전 진행 코루틴을 시작합니다.
        StartCoroutine(DungeonFlow());
    }

    // 던전의 페이즈 진행을 관리하는 코루틴
    IEnumerator DungeonFlow()
    {
        // 1 페이즈: Point 1에서 3마리 소환
        Debug.Log("== 던전 시작: 1 페이즈 진입 ==");
        SpawnEnemies(spawnPoints[0], 3);
        yield return new WaitForSeconds(timeBetweenPhases);

        // 2 페이즈: Point 2에서 3마리 소환
        Debug.Log("== 2 페이즈 진입 ==");
        SpawnEnemies(spawnPoints[1], 3);
        yield return new WaitForSeconds(timeBetweenPhases);

        // 3 페이즈: Point 3에서 3마리 소환
        Debug.Log("== 3 페이즈 진입 ==");
        SpawnEnemies(spawnPoints[2], 3);
        yield return new WaitForSeconds(timeBetweenPhases);

        // 4 페이즈: 모든 3 포인트에서 각각 2마리씩 소환 (총 6마리)
        Debug.Log("== 4 페이즈 진입 (최종 웨이브) ==");
        SpawnEnemies(spawnPoints[0], 2);
        SpawnEnemies(spawnPoints[1], 2);
        SpawnEnemies(spawnPoints[2], 2);

        Debug.Log("== 모든 페이즈 소환 완료 ==");
    }

    // 지정된 위치에서 지정된 수만큼 적을 소환하는 함수
    void SpawnEnemies(Transform spawnPoint, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 무작위 오프셋 계산 (X와 Z축에만 적용)
            // -maxSpawnOffset 부터 +maxSpawnOffset 사이의 무작위 값
            float offsetX = Random.Range(-maxSpawnOffset, maxSpawnOffset);
            float offsetZ = Random.Range(-maxSpawnOffset, maxSpawnOffset);

            // 소환 지점 위치에 오프셋을 더합니다. (Y축은 유지)
            Vector3 spawnPosition = spawnPoint.position + new Vector3(offsetX, 0f, offsetZ);

            // 새로운 위치와 회전을 사용하여 적을 생성합니다.
            Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
        }
        Debug.Log(spawnPoint.name + "에서 적 " + count + "마리 소환 완료.");
    }
}