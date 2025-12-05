using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    // 싱글턴 패턴을 위한 인스턴스
    public static SceneManager Instance { get; private set; }

    [Header("Managed Objects")]
    // 이 변수에 플레이어 오브젝트의 참조가 저장됩니다.
    private GameObject player;

    private void Awake()
    {
        // 싱글턴 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            // 씬 전환 시 이 매니저 오브젝트가 파괴되지 않도록 설정합니다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Portal 스크립트로부터 호출되는 공개 메서드
    public void TravelToScene(string targetScene, GameObject playerToMove)
    {
        this.player = playerToMove;

        // 플레이어 오브젝트를 다음 씬에서도 파괴되지 않도록 설정합니다.
        DontDestroyOnLoad(player);

        // 씬 로드 및 플레이어 이동 코루틴 시작
        StartCoroutine(LoadSceneAndRelocatePlayer(targetScene));
    }

    private IEnumerator LoadSceneAndRelocatePlayer(string targetScene)
    {
        // 씬 로드
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetScene);

        // 로딩 완료까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬 로딩 완료 후, 새 씬의 오브젝트들이 완전히 초기화될 수 있도록 한 프레임을 대기합니다.
        yield return null;

        // 씬이 완전히 로드된 후 SpawnPoint 찾기
        // 주의: 이 오브젝트는 다음 씬의 최상위 계층에 있으며, 이름이 "SpawnPoint"여야 합니다.
        GameObject spawnPoint = GameObject.Find("SpawnPoint");

        if (spawnPoint != null)
        {
            // 디버그 로그: SpawnPoint 찾음 및 좌표 출력
            Vector3 spawnPosition = spawnPoint.transform.position;
            Debug.Log($"SceneManager: SpawnPoint를 찾았습니다! 위치: ({spawnPosition.x:F2}, {spawnPosition.y:F2}, {spawnPosition.z:F2})");

            // 디버그 로그: 플레이어 위치 이동 전 알림
            Debug.Log("SceneManager: 플레이어 위치를 SpawnPoint로 이동시킵니다.");

            // 플레이어의 위치와 회전을 SpawnPoint 값으로 설정
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }
        else
        {
            // 디버그 로그: SpawnPoint 찾기 실패 경고
            Debug.LogWarning("SceneManager: 다음 씬 (" + targetScene + ") 에서 'SpawnPoint' 오브젝트를 찾지 못했습니다. 플레이어 위치 초기화 실패.");
        }
    }
}