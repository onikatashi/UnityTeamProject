using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    // 인스펙터에 연결할 플레이어 프리팹
    public GameObject playerPrefab;

    void Awake()
    {
        // 로드 매니저 자신은 DDOL을 사용하지 않습니다. 씬 로드 시 파괴/재생성됩니다.
        Debug.Log($"디버그: SceneLoadManager '{gameObject.name}'이(가) 씬 '{SceneManager.GetActiveScene().name}'에서 로드됨.");
    }

    void Start()
    {
        // Awake 후 Start에서 플레이어 초기화 로직을 실행합니다.
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        // 1. 필수 검사: 플레이어 프리팹
        if (playerPrefab == null)
        {
            Debug.LogError("오류: Player Prefab이 SceneLoadManager 인스펙터에 연결되지 않았습니다.");
            return;
        }

        // 2. 필수 검사: 플레이어 레이어 (Layer 3)
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogError("오류: 유니티 레이어 설정에 'Player' 레이어가 존재하지 않습니다. 레이어 검사를 수행할 수 없습니다.");
            return;
        }

        Debug.Log($"디버그: 플레이어 식별 레이어 ({LayerMask.LayerToName(playerLayer)})로 DontDestroyOnLoad 플레이어를 검색합니다.");

        // 3. DDOL 플레이어 검색
        GameObject existingPlayer = null;

        // FindObjectsOfType은 현재 씬뿐만 아니라 DontDestroyOnLoad 그룹의 오브젝트도 검색합니다.
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.layer == playerLayer)
            {
                existingPlayer = go;
                Debug.Log($"디버그: Layer '{LayerMask.LayerToName(playerLayer)}'를 가진 오브젝트 '{existingPlayer.name}' 발견.");
                break;
            }
        }

        // 4. SpawnPoint 찾기 (생성/이동 위치)
        Vector3 spawnPosition = Vector3.zero;
        GameObject spawnPoint = GameObject.Find("SpawnPoint");

        if (spawnPoint == null)
        {
            Debug.LogError("오류: 'SpawnPoint' 오브젝트를 씬에서 찾을 수 없습니다. 플레이어가 (0, 0, 0)에 소환되거나 이동됩니다.");
        }
        else
        {
            spawnPosition = spawnPoint.transform.position;
            Debug.Log($"디버그: 'SpawnPoint'를 위치 {spawnPosition}에서 찾았습니다.");
        }


        // =========================================================
        // 5. 플레이어 생성/이동 로직 분기
        // =========================================================

        if (existingPlayer == null)
        {
            // Case A: DDOL 플레이어가 없을 때 (주로 최초 씬 진입)
            Debug.Log("디버그: DontDestroyOnLoad된 플레이어를 찾지 못했습니다. 새 플레이어를 생성합니다.");

            // 새 플레이어 생성
            GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            // 새 플레이어를 다음 씬으로 유지되도록 DDOL 설정
            DontDestroyOnLoad(newPlayer);

            Debug.Log($"디버그: 새 플레이어 '{newPlayer.name}' 생성 완료 및 DontDestroyOnLoad 설정됨.");
            Debug.Log($"디버그: 새 플레이어는 SpawnPoint 위치 {spawnPosition}에 생성되었습니다.");
        }
        else
        {
            // Case B: DDOL 플레이어가 있을 때 (씬 전환)
            Debug.Log($"디버그: DontDestroyOnLoad된 기존 플레이어 '{existingPlayer.name}'을(를) 찾았습니다. 새 플레이어를 생성하지 않고 기존 플레이어를 유지합니다.");

            // 기존 플레이어를 SpawnPoint로 순간이동
            existingPlayer.transform.position = spawnPosition;

            Debug.Log($"디버그: 기존 플레이어를 SpawnPoint 위치 {spawnPosition}로 순간이동 완료.");
        }
    }
}