using System.Collections.Generic;
using UnityEngine;

public class DungeonMaker : MonoBehaviour
{
    //노드 생성설정
    private int maxFloor = 8;
    private int maxColumn = 6;
    private int startNodeCountLimit = 3;



    //Button의 Type에 따른 Sprite연결을 위한 Dictionary
    private Dictionary<Enums.RoomType, Sprite> iconSpriteDictionary;

    public Sprite normalSprite;
    public Sprite eliteSprite;
    public Sprite shopSprite;
    public Sprite restSprite;
    public Sprite forgeSprite;



    //라인 처리 스크립트 inspector창에서 연결.
    public LineDrawer lineDrawer;

    //시작노드 정보(층수,열, 사용하는가?, RoomType,다음노드 정보)
    private List<NodeButton> startNodes = new List<NodeButton>();
    
    //룸 타입 스크립트에서 정보 가져오기.
    public RoomTypeData roomTypeData;

    //버튼 프리팹 연결 및 생성위치.
    public GameObject roomButtonPrefab;
    public Transform mapParent;

    //던전 버튼의 층수,열의 정보.;
    private NodeButton[,] dungeonButtons;

 
    //---------------------------------------------------------------------------------------------

    void Start()
    {
        iconSpriteDictionary = new Dictionary<Enums.RoomType, Sprite>()
        {
            { Enums.RoomType.Normal, normalSprite },
            { Enums.RoomType.Elite, eliteSprite },
            { Enums.RoomType.Shop, shopSprite },
            { Enums.RoomType.Rest, restSprite },
            { Enums.RoomType.Forge, forgeSprite }
        };
        //NodeButton타입을 가진 2차원 배열 생성
        dungeonButtons = new NodeButton[maxFloor, maxColumn];
        
        //
        GenerateDungeon();
       
        ConnectNodes_Optimized();
        lineDrawer.DrawAllConnections(dungeonButtons, maxFloor, maxColumn);
        PrintDungeonToConsole();
    }

    //---------------------------------------------------------------------------------------------

    //노드 생성부---------------------------------------------------------------------------------------------
    private void GenerateDungeon()
    {
        // [0,0노드]로 부터 위치 조정
        Vector2 startPos = new Vector2(-300f, -350f);
        float xSpacing = 100f;
        float ySpacing = 100f;

        //2차원 배열을 이용한 랜덤Type 노드 생성.
        for (int floor = 0; floor < maxFloor; floor++)
        {
            for (int col = 0; col < maxColumn; col++)
            {
                
                Enums.RoomType type;
                type = roomTypeData.GetRoomType(floor, col);
                
                    
                GameObject nodePrefab = Instantiate(roomButtonPrefab, mapParent);
                NodeButton nodeButton = nodePrefab.GetComponent<NodeButton>();

                if (nodeButton == null)
                {
                    Debug.LogError("RoomButton Prefab에 NodeButton 스크립트가 없습니다!");
                    continue;
                }

                //NodeButton.cs에 각 노드 정보 저장.----------------------------------------------------------
                nodeButton.floor = floor;
                nodeButton.col = col;
                nodeButton.isAvailable = (type != Enums.RoomType.None);//RoomType이 None이 아니면 True저장.

                //NodeButton.cs - SetRoomType() 버튼에 이미지 붙이기.
                nodeButton.SetRoomType(type, type != Enums.RoomType.None ? iconSpriteDictionary[type] : null);

                dungeonButtons[floor, col] = nodeButton;

                //노드 일정간격 배치.
                RectTransform nodeTransform = nodePrefab.GetComponent<RectTransform>();
                nodeTransform.anchoredPosition = new Vector2(startPos.x + col * xSpacing, startPos.y + floor * ySpacing);

                //---------------------------------------------------------------------------------------------
            }
        }
        //모든 노드 생성 후 선택한 층 [개수 제한].
        LimitFloorNodeCount(0, startNodeCountLimit);
    }

    /// <summary>
    /// 지정한 층의 노드 개수 조정
    /// </summary>
    /// <param name="floor">층수 선택</param>
    /// <param name="limitCount">총 개수 선택</param>
    private void LimitFloorNodeCount(int floor, int limitCount)
    {
        
        List<NodeButton> activeNode = new List<NodeButton>();

        // 1. 해당 층에서 현재 활성 노드 수집
        for (int col = 0; col < maxColumn; col++)
        {
            NodeButton node = dungeonButtons[floor, col];

            if (node != null && node.isAvailable)
                activeNode.Add(node);
        }

        // 남길 개수보다 작으면 아무 작업 안함
        if (activeNode.Count <= limitCount)
            return;

        // 2. 남길 개수만큼 랜덤으로 선택
        List<NodeButton> randomSelecrtNode = new List<NodeButton>(); 
        for (int currentCount = 0; currentCount < limitCount; currentCount++)
        {
            //활성노드 중 하나 선택
            int selectNode = Random.Range(0, activeNode.Count);
            randomSelecrtNode.Add(activeNode[selectNode]);
            
            //선택된 노드는 activeNode리스트에서 제거하여 다시 선택 금지.
            activeNode.RemoveAt(selectNode);
        }

        // 3. 선택되지 않은 노드는 모두 None 처리
        foreach (NodeButton notChoiceNode in activeNode)
        {
            notChoiceNode.isAvailable = false;
            notChoiceNode.SetRoomType(Enums.RoomType.None, null);
        }
    }

    //-------------------------------------------------------------------------------------------------------

    //라인 생성부---------------------------------------------------------------------------------------------

    //노드 라인연결 로직.
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
    //격리 노드 연결시키기.
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
    //다음 노드 확장 검색.
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
    //+-1안쪽 없을 때 확장로직.
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
    //가까운 이전노드 검색.
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
    //가까운 다음 노드 검색.
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
