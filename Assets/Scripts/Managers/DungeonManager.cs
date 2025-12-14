using UnityEngine;
using static Enums;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    // [DungeonMaker 연동 관련 주석 시작]
    private const string ROOM_TYPE_PREF_KEY = "SelectedRoomType";
    private const string THEME_PREF_KEY = "SelectedTheme";
    // [DungeonMaker 연동 관련 주석 끝]

    [Header("현재 방 타입 (DungeonMaker 혹은 인스펙터 설정)")]
    public RoomType roomType;

    [Header("연결된 스폰 매니저")]
    public SpawnManager spawnManager;

    [Header("Reward Slot 1")]
    public GameObject reward1Prefab;
    public Transform reward1SpawnPoint;

    [Header("Reward Slot 2")]
    public GameObject reward2Prefab;
    public Transform reward2SpawnPoint;

    [Header("Reward Slot 3")]
    public GameObject reward3Prefab;
    public Transform reward3SpawnPoint;


    [Header("포탈 프리팹 및 스폰 위치")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;

    [Header("테마 설정")]
    public bool isRandomTheme = false;
    public DungeonTheme currentTheme;

    [Header("테마 그룹들 (씬에서 직접 드래그)")]
    public GameObject grassGroup;
    public GameObject snowGroup;
    public GameObject desertGroup;
    public GameObject lavaGroup;



    private void Start()
    {
        // 1. DungeonMaker에서 선택된 룸 타입 불러오기 및 반영
        LoadRoomTypeFromDungeonMaker();

        // 잘못된 씬 로드 오류 체크: Shop, Rest, Forge 타입은 Dungeon 씬에 로드되면 안됩니다.
        if (roomType == RoomType.Shop || roomType == RoomType.Rest || roomType == RoomType.Forge)
        {
            // 요청된 오류 메시지 출력
            Debug.LogError("맵 타입이 Shop, Rest, Forge이나 RestRoom이 아닌 Dungeon 씬이 열렸습니다.");

            // 이 스크립트의 나머지 로직(스폰, 테마 적용 등)이 실행되지 않도록 컴포넌트를 비활성화합니다.
            this.enabled = false;
            return;
        }

        // 2. 던전 테마 설정
        SetDungeonTheme();
        Debug.Log("선택된 테마: " + currentTheme);
        ApplyTheme(currentTheme);

        // 3. Normal, Elite, Boss 일 때만 스폰 시작
        if (roomType == RoomType.Normal || roomType == RoomType.Elite || roomType == RoomType.Boss)
        {
            // [DungeonMaker 연동 관련 주석 시작]
            // Boss 방의 경우, 보스 처치 후 특수 보상/연출 등이 필요할 수 있습니다.
            // [DungeonMaker 연동 관련 주석 끝]

            if (spawnManager != null)
            {
                spawnManager.OnAllEnemiesCleared += HandleRoomCleared;
                // 수정된 부분: 현재 룸 타입을 StartSpawning에 전달
                spawnManager.StartSpawning(roomType);
            }
            else
            {
                Debug.LogError("SpawnManager가 연결되지 않았습니다. 몬스터 스폰을 시작할 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// PlayerPrefs에서 룸 타입을 불러와 반영하거나, 실패 시 현재 인스펙터 값을 유지합니다.
    /// </summary>
    private void LoadRoomTypeFromDungeonMaker()
    {
        if (PlayerPrefs.HasKey(ROOM_TYPE_PREF_KEY))
        {
            int savedRoomTypeInt = PlayerPrefs.GetInt(ROOM_TYPE_PREF_KEY);
            if (System.Enum.IsDefined(typeof(RoomType), savedRoomTypeInt))
            {
                roomType = (RoomType)savedRoomTypeInt;
                Debug.Log($"DungeonMaker에서 RoomType을 불러왔습니다: {roomType}");
            }
            else
            {
                Debug.LogWarning($"DungeonMaker에서 불러온 룸 타입 값 ({savedRoomTypeInt})이 정의되지 않았습니다. 인스펙터 설정을 사용합니다: {roomType}");
            }

            PlayerPrefs.DeleteKey(ROOM_TYPE_PREF_KEY);
        }
        else
        {
            Debug.Log($"DungeonMaker 룸 타입 키 ({ROOM_TYPE_PREF_KEY})를 찾을 수 없습니다. 인스펙터 설정을 사용합니다: {roomType}");
        }
    }

    /// <summary>
    /// 테마 설정에 따라 현재 던전 테마를 결정합니다.
    /// </summary>
    private void SetDungeonTheme()
    {
        // 1. DungeonMaker에서 테마를 설정했는지 확인 (최우선)
        if (PlayerPrefs.HasKey(THEME_PREF_KEY))
        {
            int savedThemeInt = PlayerPrefs.GetInt(THEME_PREF_KEY);
            if (System.Enum.IsDefined(typeof(DungeonTheme), savedThemeInt))
            {
                currentTheme = (DungeonTheme)savedThemeInt;
                Debug.Log($"DungeonMaker에서 Theme을 불러와 설정했습니다: {currentTheme}");
                PlayerPrefs.DeleteKey(THEME_PREF_KEY);
                return;
            }
            PlayerPrefs.DeleteKey(THEME_PREF_KEY);
        }

        // 2. 랜덤 테마를 선택할지 여부 확인
        if (isRandomTheme)
        {
            List<DungeonTheme> availableThemes = new List<DungeonTheme>();

            if (grassGroup != null) availableThemes.Add(DungeonTheme.Grass);
            if (snowGroup != null) availableThemes.Add(DungeonTheme.Snow);
            if (desertGroup != null) availableThemes.Add(DungeonTheme.Desert);
            if (lavaGroup != null) availableThemes.Add(DungeonTheme.Lava);

            if (availableThemes.Count > 0)
            {
                int randomIndex = Random.Range(0, availableThemes.Count);
                currentTheme = availableThemes[randomIndex];
                Debug.Log($"랜덤 테마가 선택되었습니다: {currentTheme} (후보: {availableThemes.Count}개)");
            }
            else
            {
                Debug.LogWarning("씬에 연결된 테마 그룹이 없습니다. 인스펙터의 currentTheme 기본값을 사용합니다.");
            }
        }
        // 3. (DungeonMaker 설정 없음) && (isRandomTheme false 혹은 후보 없음)인 경우 인스펙터 값을 사용
    }



    /// <summary>
    /// 테마 그룹을 활성/비활성 한다.
    /// </summary>
    private void ApplyTheme(DungeonTheme theme)
    {
        DisableAllThemes();

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

                // [DungeonMaker 연동 관련 주석 시작]
                // Theme.None 등 예상치 못한 테마에 대한 기본 동작을 정의할 수 있습니다.
                // [DungeonMaker 연동 관련 주석 끝]
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

        SpawnAllRewards();
        SpawnPortal();

        if (spawnManager != null)
        {
            spawnManager.OnAllEnemiesCleared -= HandleRoomCleared;
        }
    }

    /// <summary>
    /// 설정된 3개의 리워드 슬롯을 확인하고 각각 스폰합니다.
    /// </summary>
    private void SpawnAllRewards()
    {
        int rewardsSpawned = 0;

        if (SpawnSingleReward(reward1Prefab, reward1SpawnPoint, 1))
            rewardsSpawned++;

        if (SpawnSingleReward(reward2Prefab, reward2SpawnPoint, 2))
            rewardsSpawned++;

        if (SpawnSingleReward(reward3Prefab, reward3SpawnPoint, 3))
            rewardsSpawned++;

        Debug.Log($"총 {rewardsSpawned}개의 리워드가 생성되었습니다.");
    }

    /// <summary>
    /// 개별 리워드를 스폰합니다.
    /// </summary>
    private bool SpawnSingleReward(GameObject prefab, Transform spawnPoint, int slotIndex)
    {
        if (prefab != null && spawnPoint != null)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"리워드 {slotIndex} 생성 완료: {prefab.name}");
            return true;
        }
        else if (prefab != null || spawnPoint != null)
        {
            Debug.LogWarning($"리워드 {slotIndex}의 프리팹 또는 스폰 위치 중 하나만 설정되었습니다. 스폰을 건너뜁니다.");
        }
        return false;
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