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

        ConnectNodes();
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
    private void ConnectNodes()
    {
        //startNodes안에있는 노드의 값을 startNode에 입력
        foreach (var startNode in startNodes)
        {
            //현재 사용중인 노드 조작을 위해  NodeButton current 
            NodeButton current = startNode;
            //연속 노드 체크
            int straightCount = 1;


            //현재 노드의 floor으로 부터 maxFloor까지 순회하면서 체크
            for (int floor = current.floor; floor < maxFloor - 1; floor++)
            {
                //현재 층보다 상위 층 확인.
                int nextFloor = floor + 1;
                
                NodeButton picked = FindNextNodeWithExpandedRange(current, nextFloor);
                if (picked == null) break;

                bool isStraight = picked.col == current.col;

                // 직진 2번 이상이면 강제 좌우 이동
                if (straightCount >= 2 && isStraight)
                {
                    var sides = GetSideCandidates(current, nextFloor);
                    if (sides.Count > 0)
                    {
                        picked = sides[Random.Range(0, sides.Count)];
                        straightCount = 1;
                    }
                }
                else
                {
                    straightCount = isStraight ? straightCount + 1 : 1;
                }

                ConnectSafe(current, picked);
                current = picked;
            }
        }

        FixIsolatedNodes_Optimized();
    }

    //다음 노드 확장 검색.
    private NodeButton FindNextNodeWithExpandedRange(NodeButton current, int nextFloor)
    {
        //
        for (int checkrange = 1; checkrange < maxColumn; checkrange++)
        {
            //Column의 검색 구간 최소, 최대 범위 설정.
            int startCol = Mathf.Max(0, current.col - checkrange);
            int endCol = Mathf.Min(maxColumn - 1, current.col + checkrange);

            //범위 설정.
            NodeButton found = FindNodeInRange(nextFloor, startCol, endCol, current.transform, true);
            if (found != null) return found;
        }
        return null;
    }



    //격리 노드 연결시키기.
    private void FixIsolatedNodes_Optimized()
    {
        for (int f = 0; f < maxFloor; f++)
        {
            for (int c = 0; c < maxColumn; c++)
            {
                NodeButton node = dungeonButtons[f, c];
                if (node == null || !node.isAvailable) continue;

                if (node.prevNodes.Count == 0 && f > 0)
                {
                    NodeButton prev = FindNearestNode(f, c, true);
                    if (prev != null) ConnectSafe(prev, node);
                }

                if (node.nextNodes.Count == 0 && f < maxFloor - 1)
                {
                    NodeButton next = FindNearestNode(f, c, false);
                    if (next != null) ConnectSafe(node, next);
                }
            }
        }
    }
    
    //+-1안쪽 없을 때 확장로직.

    private readonly List<NodeButton> sideList = new List<NodeButton>(2);

    private List<NodeButton> GetSideCandidates(NodeButton current, int nextFloor)
    {
        sideList.Clear();

        int col = current.col;

        if (col - 1 >= 0)
        {
            var left = dungeonButtons[nextFloor, col - 1];
            if (left != null && left.isAvailable) sideList.Add(left);
        }

        if (col + 1 < maxColumn)
        {
            var right = dungeonButtons[nextFloor, col + 1];
            if (right != null && right.isAvailable) sideList.Add(right);
        }

        return sideList;
    }

    private NodeButton FindNearestNode(int floor, int col, bool isPrev)
    {
        int targetFloor = isPrev ? floor - 1 : floor + 1;
        if (targetFloor < 0 || targetFloor >= maxFloor) return null;

        NodeButton origin = dungeonButtons[floor, col];
        if (origin == null) return null;

        return FindNodeInRange(
            targetFloor,
            0,
            maxColumn - 1,
            origin.transform,
            true
        );
    }

    private NodeButton FindNodeInRange(int targetFloor,int startCol, int endCol,Transform origin,bool useDistance)
    {
        NodeButton best = null;
        float bestDist = float.MaxValue;

        if (targetFloor < 0 || targetFloor >= maxFloor)
            return null;

        bool leftToRight = Random.value < 0.5f;

        if (leftToRight)
        {
            for (int c = startCol; c <= endCol; c++)
                Evaluate(c);
        }
        else
        {
            for (int c = endCol; c >= startCol; c--)
                Evaluate(c);
        }

        return best;

        void Evaluate(int c)
        {
            var candidate = dungeonButtons[targetFloor, c];
            if (candidate == null || !candidate.isAvailable) return;

            if (!useDistance)
            {
                best = candidate;
                return;
            }

            // 거리 비교를 위한 sqrMagnitude 적용
            Vector2 diff = (Vector2)candidate.transform.position - (Vector2)origin.position;
            float dist = diff.sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                best = candidate;
            }
        }
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
