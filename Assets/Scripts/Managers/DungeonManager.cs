using UnityEngine;
using static Enums;

public class DungeonManager : MonoBehaviour
{
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

    [Header("테마 그룹들 (씬에서 직접 드래그)")]
    public GameObject grassGroup;
    public GameObject snowGroup;
    public GameObject desertGroup;
    public GameObject lavaGroup;



    private void Start()
    {
        Debug.Log("선택된 테마: " + currentTheme);

        ApplyTheme(currentTheme);

        // Normal 또는 Elite 일 때만 스폰 시작
        if (roomType == RoomType.Normal || roomType == RoomType.Elite)
        {
            spawnManager.OnAllEnemiesCleared += HandleRoomCleared;
            spawnManager.StartSpawning();
        }
    }




    /// <summary>
    /// 테마 그룹을 활성/비활성 한다.
    /// </summary>
    private void ApplyTheme(DungeonTheme theme)
    {
        // 먼저 전부 비활성화
        DisableAllThemes();

        // 현재 테마만 활성화
        switch (theme)
        {
            case DungeonTheme.Grass:
                if (grassGroup) grassGroup.SetActive(true);
                break;

            case DungeonTheme.Snow:
                if (snowGroup) snowGroup.SetActive(true);
                break;

            case DungeonTheme.Desert:
                if (desertGroup) desertGroup.SetActive(true);
                break;

            case DungeonTheme.Lava:
                if (lavaGroup) lavaGroup.SetActive(true);
                break;
        }

        Debug.Log("테마 적용 완료: " + theme);
    }



    /// <summary>
    /// 모든 테마 그룹 비활성화
    /// </summary>
    private void DisableAllThemes()
    {
        if (grassGroup) grassGroup.SetActive(false);
        if (snowGroup) snowGroup.SetActive(false);
        if (desertGroup) desertGroup.SetActive(false);
        if (lavaGroup) lavaGroup.SetActive(false);
    }




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
