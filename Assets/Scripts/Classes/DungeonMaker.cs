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

        GenerateDungeon();
        GenerateNodeConnections();
        lineDrawer.DrawAllConnections(dungeonButtons, maxFloor, maxColumn);
        PrintDungeonToConsole();
    }

    //---------------------------------------------------------------------------------------------

    //노드 생성부---------------------------------------------------------------------------------------------
    private void GenerateDungeon()
    {
        // [0,0노드]로 부터 위치 조정
        Vector2 startPos = new Vector2(-250f, -350f);
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
    private void GenerateNodeConnections()
    {
        //startNodes안에있는 노드의 값을 startNode에 입력
        foreach (var entryNode in startNodes)
        {
            //현재 사용중인 노드 조작을 위해  NodeButton current 
            NodeButton currentNode = entryNode;
            //연속 노드 체크
            int straightCount = 1;


            //현재 노드의 floor으로 부터 maxFloor까지 순회하면서 체크
            for (int floor = currentNode.floor; floor < maxFloor - 1; floor++)
            {
                //다음 층 확인.
                int nextFloor = floor + 1;
                
                //다음 층의 연결 가능 노드 찾기(None노드 인가? 체크)
                NodeButton picked = FindNextAvailableNode(currentNode, nextFloor);
                if (picked == null) break;

                //선택한 노드와 현재 노드가 같은 열인가?
                bool isStraight = picked.col == currentNode.col;

                // 직진 3번 이상이면 강제 좌우 이동 아니면 초기화.
                if (straightCount >= 2 && isStraight)
                {
                    //GetSideCandidates를 이용해 다음 층(nextFloor)의 좌우 노드 값 가져오기.
                    var sides = GetSideCandidates(currentNode, nextFloor);
                    
                    //좌우 노드가 한개라도 있으면  둘중하나 연결.
                    if (sides.Count > 0)
                    {
                        picked = sides[Random.Range(0, sides.Count)];
                        // 틀어졌으므로 연속연결 초기화
                        straightCount = 1;
                    }
                }
                else
                {
                    straightCount = isStraight ? straightCount + 1 : 1;
                }
                //안전 연결을 위한 코드.
                ConnectSafe(currentNode, picked);
                //다음 노드(picked)를 currentNode에 넣어 해당 과정 반복.
                currentNode = picked;
            }
        }

        ConnectIsolatedNodes();
    }

    /// <summary>
    /// 다음 층에서 노드 찾기
    /// </summary>
    /// <param name="currentNode">현재 노드</param>
    /// <param name="nextFloor">검색할 다음 층 </param>
    /// <returns></returns>
    private NodeButton FindNextAvailableNode(NodeButton currentNode, int nextFloor)
    {
        
        for (int checkRange = 1; checkRange < maxColumn; checkRange++)
        {
            //Column의 검색 구간 최소, 최대 범위 설정.
            int startCol = Mathf.Max(0, currentNode.col - checkRange);
            int endCol = Mathf.Min(maxColumn - 1, currentNode.col + checkRange);

            //범위 설정.
            NodeButton found = FindNodeWithinRange(nextFloor, startCol, endCol, currentNode.transform, true);
            if (found != null) return found;
        }
        return null;
    }

    
    private NodeButton FindNodeWithinRange(int targetFloor, int startCol, int endCol, Transform origin, bool useDistance)
    {
        //노드 
        NodeButton bestNode = null;
        float minDistSqr = float.MaxValue;

        if (targetFloor < 0 || targetFloor >= maxFloor)
            return null;

        // 랜덤 방향 (50%확률로 좌,우 -> 좌측 쏠림 완화.)
        bool searchLeftToRight = Random.value < 0.5f;

        //해당 층의 노드 연결 후보 평가.
        if (searchLeftToRight)
        {
            for (int columRange = startCol; columRange <= endCol; columRange++) EvaluateCandidate(columRange);
        }
        else
        {
            for (int columRange = endCol; columRange >= startCol; columRange--) EvaluateCandidate(columRange);
        }

        return bestNode;

        void EvaluateCandidate(int selectColum)
        {
            //다음층(targetFloor)의 해당 열의 후보노드가 쓸만한가?
            var candidateNode = dungeonButtons[targetFloor, selectColum];
            if (candidateNode == null || !candidateNode.isAvailable) return;

            // 거리계산 모드 체크 : 거리 계산을 사용하지 않는 모드일 경우, 첫 번째 유효 노드를 바로 선택 후 종료.
            if (!useDistance)
            {
                bestNode = candidateNode;
                return;
            }

            // 거리 비교 (sqrMagnitude)
            Vector2 delta = ((Vector2)candidateNode.transform.position - (Vector2)origin.position);
            float distSqr = delta.sqrMagnitude;

            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                bestNode = candidateNode;
            }
        }
    }

    // sideList를 미리 2개 지정해 놓아서 좌우Node체크용으로 재활용(생성 -> 제거 리소스 아끼기)
    private readonly List<NodeButton> sideList = new List<NodeButton>(2);
   
    private List<NodeButton> GetSideCandidates(NodeButton currentNode, int nextFloor)
    {
        //재활용하기 위해 Clear
        sideList.Clear();

        //현재 노드의 열을 넣고
        int colum = currentNode.col;
        //다음층 의 좌우 확인.
        if (colum - 1 >= 0)
        {
            var left = dungeonButtons[nextFloor, colum - 1];
            if (left != null && left.isAvailable) sideList.Add(left);
        }

        if (colum + 1 < maxColumn)
        {
            var right = dungeonButtons[nextFloor, colum + 1];
            if (right != null && right.isAvailable) sideList.Add(right);
        }

        return sideList;
    }

    //격리 노드 연결시키기.
    private void ConnectIsolatedNodes()
    {
        //모든 노드 순회하며 확인.
        for (int floor = 0; floor < maxFloor; floor++)
        {
            for (int colum = 0; colum < maxColumn; colum++)
            {

                NodeButton checkNode = dungeonButtons[floor, colum];
                
                //현재 노드가 없으면 지나가기.
                if (checkNode == null || !checkNode.isAvailable) continue;

                //현재 노드가 시작노드보다 높고 이전노드 연결이 없을 경우.
                if (checkNode.prevNodes.Count == 0 && floor > 0)
                {
                    NodeButton prev = FindNearestNode(floor, colum, true);
                    if (prev != null) ConnectSafe(prev, checkNode);
                }

                //현재 노드가 보스방직전보다 낮고고 다음노드와 연결이 없을 경우.
                if (checkNode.nextNodes.Count == 0 && floor < maxFloor - 1)
                {
                    NodeButton next = FindNearestNode(floor, colum, false);
                    if (next != null) ConnectSafe(checkNode, next);
                }
            }
        }
    }

    /// <summary>
    /// 가까운 노드 검색 공용함수 (위, 아래 양방향)
    /// </summary>
    /// <param name="floor">가까운 노드의 층</param>
    /// <param name="col">가까운 노드의 열</param>
    /// <param name="isPrev"> [True이면 Pre floor체크,False이면 Next floor체크] => 리소스 아끼기 위해 Bool사용</param>
    /// <returns></returns>
    private NodeButton FindNearestNode(int floor, int col, bool isPrev)
    {
        int targetFloor = isPrev ? floor - 1 : floor + 1;
        
        //노드들이 범위를 벗어나면 Null
        if (targetFloor < 0 || targetFloor >= maxFloor) return null;

        //내부에 있으면 origin위치 설정.
        NodeButton origin = dungeonButtons[floor, col];
        if (origin == null) return null;

        return FindNodeWithinRange(targetFloor,0,maxColumn - 1, origin.transform, true );
    }

    private void ConnectSafe(NodeButton currentNode, NodeButton nextNode)
    {
        //둘 중 하나라도 null이면 함수 종료.
        if (currentNode == null || nextNode == null) return;

        //[중복연결 방지]
        // currentNode노드의 다음 연결nextNode가 이미 있는지 확인.
        if (!currentNode.nextNodes.Contains(nextNode)) currentNode.nextNodes.Add(nextNode);
        //'nextNode' 노드의 이전 연결(prevNodes)에 'currentNode'이 없다면 추가.
        if (!nextNode.prevNodes.Contains(currentNode)) nextNode.prevNodes.Add(currentNode);
    }

    // 내부 지도 Type 콘솔로 확인.
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
