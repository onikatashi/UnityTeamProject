using UnityEngine;
using static Enums;
using System.Collections.Generic;

public class DungeonManager : MonoBehaviour
{
    // DungeonMaker.cs에서 다음 씬 로드 전 이 정적 변수에 값을 설정했다고 가정합니다.
    // PlayerPrefs 대신 사용되는 임시 데이터 저장소입니다.
    public static RoomType NextRoomType = RoomType.None;
    public static DungeonTheme NextDungeonTheme = DungeonTheme.None;

    [Header("현재 방 타입 (인스펙터 기본값 / DungeonMaker 전달값)")]
    [Tooltip("DungeonMaker에서 값이 전달되지 않으면 인스펙터에 지정된 값이 기본값으로 사용됩니다.")]
    public RoomType currentRoomType;

    [Header("현재 던전 테마 (인스펙터 기본값 / DungeonMaker 전달값)")]
    [Tooltip("DungeonMaker에서 값이 전달되지 않으면 인스펙터에 지정된 값이 기본값으로 사용됩니다.")]
    public DungeonTheme currentDungeonTheme;

    [Header("연결된 스폰 매니저")]
    public SpawnManager spawnManager;

    [Header("연결된 맵 메이커")]
    public MapMaker mapMaker;


    private void Awake()
    {
        // ======================= 요청하신 Fallback 로직 적용 시작 =======================
        // 1. NextRoomType이 None이 아니라면 (DungeonMaker가 설정했다면) 그 값을 사용하고,
        //    None이라면 인스펙터에 지정된 현재 값을 유지합니다.
        if (NextRoomType != RoomType.None)
        {
            currentRoomType = NextRoomType;
        }

        // 2. NextDungeonTheme이 None이 아니라면 그 값을 사용하고,
        //    None이라면 인스펙터에 지정된 현재 값을 유지합니다.
        if (NextDungeonTheme != DungeonTheme.None)
        {
            currentDungeonTheme = NextDungeonTheme;
        }
        // ======================= 요청하신 Fallback 로직 적용 끝 =========================

        // 사용 후 바로 초기화 (다음 던전 진입 시 꼬임 방지)
        NextRoomType = RoomType.None;
        NextDungeonTheme = DungeonTheme.None;
    }


    private void Start()
    {
        // 1. MapMaker에 테마 정보 전달 및 적용 요청
        if (mapMaker != null)
        {
            mapMaker.ApplyTheme(currentDungeonTheme);
        }
        else
        {
            Debug.LogError("MapMaker가 연결되지 않았습니다. 던전 테마를 적용할 수 없습니다.");
        }

        // 2. SpawnManager에 룸 타입 정보 전달 및 스폰 시작
        if (currentRoomType == RoomType.Normal || currentRoomType == RoomType.Elite || currentRoomType == RoomType.Boss)
        {
            if (spawnManager != null)
            {
                // 수정된 부분: 현재 룸 타입을 StartSpawning에 전달
                spawnManager.StartSpawning(currentRoomType);
            }
            else
            {
                Debug.LogError("SpawnManager가 연결되지 않았습니다. 몬스터 스폰을 시작할 수 없습니다.");
            }
        }
        else if (currentRoomType != RoomType.None)
        {
            Debug.Log($"룸 타입 ({currentRoomType})이 Normal, Elite, Boss가 아니므로 몬스터 스폰 로직을 건너뜁니다.");
        }
        else
        {
            Debug.LogWarning("RoomType이 None입니다. 스폰 로직을 시작할 수 없습니다. 인스펙터 설정을 확인하세요.");
        }
    }
}