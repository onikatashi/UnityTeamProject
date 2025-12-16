using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    [Header("현재 던전 저장 데이터")]
    public DungeonMapData currentDungeonData;

    [Header("현재 테마")]
    public Enums.DungeonTheme currentTheme;

    [Header("현재 선택된 룸 타입")]
    public Enums.RoomType currentRoomType;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetRandomTheme(); // 첫 진입 시 테마 설정
    }

    private void SetRandomTheme()
    {
        // 전체 테마 목록 가져와 배열로 저장.
        Enums.DungeonTheme[] themes = (Enums.DungeonTheme[])System.Enum.GetValues(typeof(Enums.DungeonTheme));

        // 배열 중 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, themes.Length);
        currentTheme = themes[randomIndex];

        Debug.Log("[DungeonManager] 초기 테마 선택: " + currentTheme);
    }

    public void SetNextTheme()
    {
        // 전체 테마 목록 가져와 배열로 저장.
        Enums.DungeonTheme[] themes = (Enums.DungeonTheme[])System.Enum.GetValues(typeof(Enums.DungeonTheme));

        // 테마 리스트 화
        List<Enums.DungeonTheme> availableThemes = new List<Enums.DungeonTheme>();

        //현재 테마 제외한 후보 생성
        for (int i = 0; i < themes.Length; i++)
        {
            if (themes[i] != currentTheme)
            {
                availableThemes.Add(themes[i]);
            }
        }

        // 예외 방지
        if (availableThemes.Count == 0)
        {
            Debug.LogWarning("[DungeonManager] 사용 가능한 다른 테마가 없습니다.");
            return;
        }

        // 랜덤으로 선택
        int randomIndex = Random.Range(0, availableThemes.Count);
        currentTheme = availableThemes[randomIndex];

        Debug.Log("[DungeonManager] 새로운 테마 선택: " + currentTheme);
    }

    // (외부 참조용) 변수 GetCurrentTheme()시 현재 정해진 테마 값 반환.
    public Enums.DungeonTheme GetCurrentTheme()
    {
        return currentTheme;
    }


    // (외부 참조용) 현재 룸타입에 대하여 반환하기.
    public Enums.RoomType GetCurrentRoomType()
    {
        return currentRoomType;
    }

    // 노드 클릭시 룸 타입 값 설정
    public void SetCurrentRoomType(Enums.RoomType roomType)
    {
        currentRoomType = roomType;
        Debug.Log("[DungeonManager] 현재 룸 타입 설정: " + currentRoomType);
    }

    //노드 데이터 보유-------------------------------------------------------------------------

    // 새로운 던전 데이터를 저장 (Maker에서 전달받음)
    public void SaveDungeonData(DungeonMapData data)
    {
        currentDungeonData = data;
        Debug.Log("[DungeonManager] 던전 데이터 저장 완료");
    }

    // 현재 던전 데이터를 반환
    public DungeonMapData GetDungeonData()
    {
        return currentDungeonData;
    }

    // 현재 던전 데이터가 존재하는가?
    public bool HasDungeonData()
    {
        return currentDungeonData != null && currentDungeonData.nodes.Count > 0;
    }

    // 클리어, 포기, 종료 시 초기화
    public void ClearDungeonData()
    {
        currentDungeonData = null;
        Debug.Log("[DungeonManager] 던전 데이터 초기화 완료");
    }

    void Update()
    {
        // 스페이스를 누르면 Keypad1 기능 실행
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("MapDateCheckScene");
        }

        // 왼쪽 시프트를 누르면 Keypad2 기능 실행
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("DungeonMap");
        }
    }


}

[System.Serializable]
public class DungeonMapData
{
    //던전 기본 정보.
    public int maxFloor;
    public int maxColumn;
    public Enums.DungeonTheme theme;

    //[각 노드별 데이터 정보]를 리스트 형식으로 저장.
    public List<DungeonNodeData> nodes = new List<DungeonNodeData>();
}

[System.Serializable]
public class DungeonNodeData //각 노드별 데이터 정보
{
    //기본 노드 정보.
    public int floor;
    public int col;
    public Enums.RoomType roomType;
    public bool isAvailable;

    //노드의 좌표정보(Centor)
    public Vector2 uiPosition;
    //다음 노드 정보를 리스트 형식으로 저장.
    public List<Vector2Int> nextNodes = new List<Vector2Int>();
}

//public class DungeonManager : MonoBehaviour
//{
//    // DungeonMaker.cs에서 다음 씬 로드 전 이 정적 변수에 값을 설정했다고 가정합니다.
//    // PlayerPrefs 대신 사용되는 임시 데이터 저장소입니다.
//    public static RoomType NextRoomType = RoomType.None;
//    public static DungeonTheme NextDungeonTheme = DungeonTheme.None;

//    [Header("현재 방 타입 (인스펙터 기본값 / DungeonMaker 전달값)")]
//    [Tooltip("DungeonMaker에서 값이 전달되지 않으면 인스펙터에 지정된 값이 기본값으로 사용됩니다.")]
//    public RoomType currentRoomType;

//    [Header("현재 던전 테마 (인스펙터 기본값 / DungeonMaker 전달값)")]
//    [Tooltip("DungeonMaker에서 값이 전달되지 않으면 인스펙터에 지정된 값이 기본값으로 사용됩니다.")]
//    public DungeonTheme currentDungeonTheme;

//    [Header("연결된 스폰 매니저")]
//    public SpawnManager spawnManager;

//    [Header("연결된 맵 메이커")]
//    public MapMaker mapMaker;


//    private void Awake()
//    {
//        // ======================= 요청하신 Fallback 로직 적용 시작 =======================
//        // 1. NextRoomType이 None이 아니라면 (DungeonMaker가 설정했다면) 그 값을 사용하고,
//        //    None이라면 인스펙터에 지정된 현재 값을 유지합니다.
//        if (NextRoomType != RoomType.None)
//        {
//            currentRoomType = NextRoomType;
//        }

//        // 2. NextDungeonTheme이 None이 아니라면 그 값을 사용하고,
//        //    None이라면 인스펙터에 지정된 현재 값을 유지합니다.
//        if (NextDungeonTheme != DungeonTheme.None)
//        {
//            currentDungeonTheme = NextDungeonTheme;
//        }
//        // ======================= 요청하신 Fallback 로직 적용 끝 =========================

//        // 사용 후 바로 초기화 (다음 던전 진입 시 꼬임 방지)
//        NextRoomType = RoomType.None;
//        NextDungeonTheme = DungeonTheme.None;
//    }


//    private void Start()
//    {
//        // 1. MapMaker에 테마 정보 전달 및 적용 요청
//        if (mapMaker != null)
//        {
//            mapMaker.ApplyTheme(currentDungeonTheme);
//        }
//        else
//        {
//            Debug.LogError("MapMaker가 연결되지 않았습니다. 던전 테마를 적용할 수 없습니다.");
//        }

//        // 2. SpawnManager에 룸 타입 정보 전달 및 스폰 시작
//        if (currentRoomType == RoomType.Normal || currentRoomType == RoomType.Elite || currentRoomType == RoomType.Boss)
//        {
//            if (spawnManager != null)
//            {
//                // 수정된 부분: 현재 룸 타입을 StartSpawning에 전달
//                spawnManager.StartSpawning(currentRoomType);
//            }
//            else
//            {
//                Debug.LogError("SpawnManager가 연결되지 않았습니다. 몬스터 스폰을 시작할 수 없습니다.");
//            }
//        }
//        else if (currentRoomType != RoomType.None)
//        {
//            Debug.Log($"룸 타입 ({currentRoomType})이 Normal, Elite, Boss가 아니므로 몬스터 스폰 로직을 건너뜁니다.");
//        }
//        else
//        {
//            Debug.LogWarning("RoomType이 None입니다. 스폰 로직을 시작할 수 없습니다. 인스펙터 설정을 확인하세요.");
//        }
//    }
//}