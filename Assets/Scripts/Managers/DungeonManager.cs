using UnityEngine;
using static Enums;

public class DungeonManager : MonoBehaviour
{
    public enum DungeonTheme
    {
        Desert,
        Grass,
        Lava,
        Snow
    }

    [Header("현재 방 타입 (외부에서 설정됨)")]
    public RoomType roomType;

    [Header("연결된 스폰 매니저")]
    public SpawnManager spawnManager;

    [Header("보물상자 프리팹 및 스폰 위치")]
    public GameObject chestPrefab;
    public Transform chestSpawnPoint;

    [Header("포탈 프리팹 및 스폰 위치")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;

    [Header("현재 던전 테마")]
    public DungeonTheme currentTheme;


    private void Start()
    {
        // 테마 랜덤 지정
        currentTheme = (DungeonTheme)Random.Range(0, 4);
        Debug.Log("선택된 테마: " + currentTheme);

        // Normal 또는 Elite 일 때만 스폰 시작
        if (roomType == RoomType.Normal || roomType == RoomType.Elite)
        {
            spawnManager.OnAllEnemiesCleared += HandleRoomCleared; // 이벤트 연결
            spawnManager.StartSpawning();
        }
    }


    /// <summary>
    /// 스폰 매니저가 "모든 적 제거" 이벤트를 보내면 호출됨
    /// </summary>
    private void HandleRoomCleared()
    {
        Debug.Log("== 던전 방 클리어 ==");

        SpawnChest();
        SpawnPortal();
    }


    private void SpawnChest()
    {
        if (chestPrefab != null && chestSpawnPoint != null)
        {
            Instantiate(chestPrefab, chestSpawnPoint.position, chestSpawnPoint.rotation);
            Debug.Log("보물상자 생성 완료");
        }
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
