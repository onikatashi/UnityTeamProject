using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 1. 방 종류 확률에 따라 맵 생성
/// </summary>


public class DungeonMaker : MonoBehaviour
{
    public int maxFloor = 7;   // 총 층 수
    public int maxColumn = 5;  // 1층당 노드 개수
    public int startNodeCount = 1; // 시작노드 개수(1~4개)

    public LineDrawer lineDrawer;

    private System.Random sysRand;

    private Dictionary<Enums.RoomType, Sprite> iconDictionary;   //버튼 스프라이트용 Dictionary
    private List<NodeButton> startNodes = new List<NodeButton>();//버튼 연결선용 List

    //Inspector이용 스프라이트 연결
    public Sprite normalSprite;
    public Sprite eliteSprite;
    public Sprite shopSprite;
    public Sprite restSprite;
    public Sprite forgeSprite;

    public RoomTypeData roomTypeData;     //룸 종류

    [Header("Button Prefab & Parent")]
    public GameObject roomButtonPrefab;   // NodeButton 프리팹
    public Transform mapParent;           // 오브젝트 생성위치 조정.

    private NodeButton[,] dungeonButtons;

    void Start()
    {
        iconDictionary = new Dictionary<Enums.RoomType, Sprite>()
        {
            { Enums.RoomType.Normal, normalSprite },
            { Enums.RoomType.Elite, eliteSprite },
            { Enums.RoomType.Shop, shopSprite },
            { Enums.RoomType.Rest, restSprite },
            { Enums.RoomType.Forge, forgeSprite }
        };

        dungeonButtons = new NodeButton[maxFloor, maxColumn];
        //던전 생성
        GenerateDungeon();
        SelectStartNodes();
        //라인그리기
        ConnectNodes_Random3Way();
        lineDrawer.DrawAllConnections(dungeonButtons, maxFloor, maxColumn);
        //체크용 콘솔 출력
        PrintDungeonToConsole();
    }

    private void GenerateDungeon()
    {
        for (int floor = 0; floor < maxFloor; floor++)
        {
            for (int col = 0; col < maxColumn; col++)
            {
                // 1️. RoomType 결정
                Enums.RoomType type = roomTypeData.GetRoomType(floor, col);

                // 2️. NodeButton 생성
                GameObject go = Instantiate(roomButtonPrefab, mapParent);
                NodeButton nodeButton = go.GetComponent<NodeButton>();

                if (nodeButton == null)
                {
                    Debug.LogError("RoomButton Prefab에 NodeButton 스크립트가 없습니다!");
                    continue;
                }

                // 3️. RoomType 적용
                nodeButton.isAvailable = type != Enums.RoomType.None; // None이면 클릭 불가


                //None일 때 아이콘 설정을 건너뛰는 부분
                if (type == Enums.RoomType.None)
                {
                    nodeButton.SetRoomType(type, null);   // 아이콘 없음
                }
                else
                {
                    nodeButton.SetRoomType(type, iconDictionary[type]); // 정상 RoomType은 Dictionary에서 스프라이트
                }
               

                // 4️. 배열에 저장
                dungeonButtons[floor, col] = nodeButton;

                // 5️. UI 위치 지정 (간단히 Grid 배치, 필요하면 GridLayout 또는 RectTransform 설정)
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(col * 100f, floor * 100f); // 예시 위치
            }
        }
    }

   
    private void SelectStartNodes()
    {
        startNodes.Clear();

        List<int> availableCols = new List<int>();
        for (int c = 0; c < maxColumn; c++)
        {
            if (dungeonButtons[0, c] != null)
                availableCols.Add(c);
        }

        int count = Mathf.Min(startNodeCount, availableCols.Count);

        // 선택 방식: 균등 분포(가운데 우선) 대신 "무작위"로 간단 구현
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, availableCols.Count);
            int col = availableCols[idx];
            availableCols.RemoveAt(idx);

            startNodes.Add(dungeonButtons[0, col]);
        }
    }

    private void ConnectNodes_Random3Way()
    {
        for (int f = 0; f < maxFloor - 1; f++)
        {
            for (int c = 0; c < maxColumn; c++)
            {
                NodeButton from = dungeonButtons[f, c];
                if (from == null) continue;

                // 연결 후보: 다음 floor에서 col-1, col, col+1
                List<NodeButton> candidates = new List<NodeButton>();

                for (int nc = c - 1; nc <= c + 1; nc++)
                {
                    if (nc < 0 || nc >= maxColumn) continue;

                    NodeButton to = dungeonButtons[f + 1, nc];
                    if (to != null)
                        candidates.Add(to);
                }

                if (candidates.Count > 0)
                {
                    NodeButton picked = candidates[Random.Range(0, candidates.Count)];
                    ConnectSafe(from, picked);
                }
            }
        }
    }
    private void ConnectSafe(NodeButton from, NodeButton to)
    {
        if (from == null || to == null) return;

        // 중복 방지
        if (!from.nextNodes.Contains(to))
            from.nextNodes.Add(to);

        if (!to.prevNodes.Contains(from))
            to.prevNodes.Add(from);
    }

    private void PrintDungeonToConsole()
    {
        Debug.Log("===== Dungeon Result =====");

        for (int floor = 0; floor < maxFloor; floor++)
        {
            string line = $"Floor {floor}: ";

            for (int col = 0; col < maxColumn; col++)
            {
                NodeButton nb = dungeonButtons[floor, col];
                line += (nb.isAvailable ? nb.CurrentRoomType.ToString() : "None").PadRight(8);
            }

            Debug.Log(line);
        }
    }

}
