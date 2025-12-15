using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapTestManager : MonoBehaviour
{
    public static MapTestManager Instance { get; private set; }

    [Header("현재 던전 저장 데이터")]
    public DungeonMapData currentDungeonData;

    [Header("현재 테마")]
    public Enums.DungeonTheme currentTheme;

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
        // 전체 테마 목록 가져오기
        Enums.DungeonTheme[] themes = (Enums.DungeonTheme[])System.Enum.GetValues(typeof(Enums.DungeonTheme));

        // 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, themes.Length);
        currentTheme = themes[randomIndex];

        Debug.Log("[DungeonManager] 초기 테마 선택: " + currentTheme);
    }

    public void SetNextTheme()
    {
        // 전체 테마 목록 가져오기
        Enums.DungeonTheme[] themes = (Enums.DungeonTheme[])System.Enum.GetValues(typeof(Enums.DungeonTheme));

        // 현재 테마 제외한 후보 생성
        List<Enums.DungeonTheme> availableThemes = new List<Enums.DungeonTheme>();
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

    public Enums.DungeonTheme GetCurrentTheme()
    {
        return currentTheme;
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
