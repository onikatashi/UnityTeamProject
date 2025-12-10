using System.Collections.Generic;
using UnityEngine;

public class DungeonMaker : MonoBehaviour
{
    private int maxFloor = 8;
    private int maxColumn = 6;
    private int startNodeCount = 3;

    public LineDrawer lineDrawer;

    private Dictionary<Enums.RoomType, Sprite> iconDictionary;
    private List<NodeButton> startNodes = new List<NodeButton>();

    public Sprite normalSprite;
    public Sprite eliteSprite;
    public Sprite shopSprite;
    public Sprite restSprite;
    public Sprite forgeSprite;

    public RoomTypeData roomTypeData;

    public GameObject roomButtonPrefab;
    public Transform mapParent;

    private NodeButton[,] dungeonButtons;
    private Dictionary<int, Enums.RoomType> forcedFloors = new Dictionary<int, Enums.RoomType>()
    {
        { 0, Enums.RoomType.Normal }
    };
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
        GenerateDungeon();
        SelectStartNodes();
        ConnectNodes_Optimized();
        lineDrawer.DrawAllConnections(dungeonButtons, maxFloor, maxColumn);
        PrintDungeonToConsole();
    }

    private void GenerateDungeon()
    {

        Vector2 startPos = new Vector2(-300f, -350f);
        float xSpacing = 100f;
        float ySpacing = 100f;


        for (int floor = 0; floor < maxFloor; floor++)
        {
            for (int col = 0; col < maxColumn; col++)
            {
                // 강제 타입이 설정된 층이면 그것으로 사용
                Enums.RoomType type;

                // 강제 타입이 설정된 층이면 그것으로 사용
                if (forcedFloors.ContainsKey(floor))
                    type = forcedFloors[floor];
                else
                    type = roomTypeData.GetRoomType(floor, col);
                GameObject go = Instantiate(roomButtonPrefab, mapParent);
                NodeButton nodeButton = go.GetComponent<NodeButton>();

                if (nodeButton == null)
                {
                    Debug.LogError("RoomButton Prefab에 NodeButton 스크립트가 없습니다!");
                    continue;
                }

                nodeButton.floor = floor;
                nodeButton.col = col;
                nodeButton.isAvailable = type != Enums.RoomType.None;
                nodeButton.SetRoomType(type, type != Enums.RoomType.None ? iconDictionary[type] : null);

                dungeonButtons[floor, col] = nodeButton;

                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startPos.x + col * xSpacing, startPos.y + floor * ySpacing);
            }
        }
    }

    private void SelectStartNodes()
    {
        // 기존 startNodes 초기화
        startNodes.Clear();

        // 0층에서 생성 가능한 컬럼 리스트
        List<int> availableCols = new List<int>();
        for (int c = 0; c < maxColumn; c++)
        {
            NodeButton node = dungeonButtons[0, c];
            if (node != null && node.isAvailable)
                availableCols.Add(c);
        }

        // startNodeCount 개수만큼 랜덤 선택
        int count = Mathf.Min(startNodeCount, availableCols.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, availableCols.Count);
            int col = availableCols[idx];
            availableCols.RemoveAt(idx);

            startNodes.Add(dungeonButtons[0, col]);
        }

        // 선택되지 않은 0층 노드는 모두 None 처리
        for (int c = 0; c < maxColumn; c++)
        {
            NodeButton node = dungeonButtons[0, c];
            if (node != null && !startNodes.Contains(node))
            {
                node.SetRoomType(Enums.RoomType.None, null);
            }
        }
    }

    private void ConnectNodes_Optimized()
    {
        foreach (var startNode in startNodes)
        {
            NodeButton current = startNode;
            int straightCount = 1;

            for (int f = current.floor; f < maxFloor - 1; f++)
            {
                int nextFloor = f + 1;
                NodeButton picked = FindNextNodeWithExpandedRange(current, nextFloor);

                if (picked == null) break;

                if (straightCount >= 2 && picked.col == current.col)
                {
                    List<NodeButton> sideCandidates = GetSideCandidates(current, nextFloor);
                    if (sideCandidates.Count > 0)
                    {
                        picked = sideCandidates[Random.Range(0, sideCandidates.Count)];
                        straightCount = 1;
                    }
                }
                else
                {
                    straightCount = (picked.col == current.col) ? straightCount + 1 : 1;
                }

                ConnectSafe(current, picked);
                current = picked;
            }
        }

        FixIsolatedNodes_Optimized();
    }

    private void FixIsolatedNodes_Optimized()
    {
        for (int f = 1; f < maxFloor; f++) // 0층 제외
        {
            for (int c = 0; c < maxColumn; c++)
            {
                NodeButton node = dungeonButtons[f, c];
                if (node == null || !node.isAvailable) continue;

                if (node.prevNodes.Count == 0)
                {
                    NodeButton prev = FindNearestPreviousNode(f, c);
                    if (prev != null) ConnectSafe(prev, node);
                }

                if (node.nextNodes.Count == 0 && f < maxFloor - 1)
                {
                    NodeButton next = FindNearestNextNode(f, c);
                    if (next != null) ConnectSafe(node, next);
                }
            }
        }
    }

    private NodeButton FindNextNodeWithExpandedRange(NodeButton current, int nextFloor)
    {
        for (int range = 1; range < maxColumn; range++)
        {
            List<NodeButton> candidates = new List<NodeButton>();
            int startCol = Mathf.Max(0, current.col - range);
            int endCol = Mathf.Min(maxColumn - 1, current.col + range);

            for (int c = startCol; c <= endCol; c++)
            {
                NodeButton to = dungeonButtons[nextFloor, c];
                if (to != null && to.isAvailable)
                    candidates.Add(to);
            }

            if (candidates.Count > 0)
            {
                candidates.Sort((a, b) => Vector2.Distance(current.transform.position, a.transform.position)
                    .CompareTo(Vector2.Distance(current.transform.position, b.transform.position)));
                return candidates[0];
            }
        }
        return null;
    }

    private List<NodeButton> GetSideCandidates(NodeButton current, int nextFloor)
    {
        List<NodeButton> sideCandidates = new List<NodeButton>();
        for (int offset = -1; offset <= 1; offset += 2)
        {
            int col = current.col + offset;
            if (col >= 0 && col < maxColumn)
            {
                NodeButton node = dungeonButtons[nextFloor, col];
                if (node != null && node.isAvailable)
                    sideCandidates.Add(node);
            }
        }
        return sideCandidates;
    }

    private NodeButton FindNearestPreviousNode(int floor, int col)
    {
        NodeButton best = null;
        float minDist = float.MaxValue;

        for (int nc = 0; nc < maxColumn; nc++)
        {
            NodeButton candidate = dungeonButtons[floor - 1, nc];
            if (candidate != null && candidate.isAvailable)
            {
                float dist = Vector2.Distance(candidate.transform.position, dungeonButtons[floor, col].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    best = candidate;
                }
            }
        }
        return best;
    }

    private NodeButton FindNearestNextNode(int floor, int col)
    {
        NodeButton best = null;
        float minDist = float.MaxValue;

        for (int nc = 0; nc < maxColumn; nc++)
        {
            NodeButton candidate = dungeonButtons[floor + 1, nc];
            if (candidate != null && candidate.isAvailable)
            {
                float dist = Vector2.Distance(candidate.transform.position, dungeonButtons[floor, col].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    best = candidate;
                }
            }
        }
        return best;
    }

    private void ConnectSafe(NodeButton from, NodeButton to)
    {
        if (from == null || to == null) return;
        if (!from.nextNodes.Contains(to)) from.nextNodes.Add(to);
        if (!to.prevNodes.Contains(from)) to.prevNodes.Add(from);
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
