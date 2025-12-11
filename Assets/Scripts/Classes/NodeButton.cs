using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 마우스 인식 관련 핸들 추가(IPointerEnterHandler, IPointerExitHandler)
/// </summary>
public class NodeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int floor; // 층 정보
    public int col;   // 컬럼 정보

    //이미지 스프라이트와 하이라이트 UI연동
    private Image icon;
    private GameObject highlight;
   

    //현재 방의 정보
    public bool isAvailable = true;
    public Enums.RoomType CurrentRoomType { get; private set; } // 현재 방 타입

    // DungeonMaker에서 연결하기 위한 그래프 구조
    public List<NodeButton> nextNodes = new List<NodeButton>();   // 아래층(다음층) 노드들
    public List<NodeButton> prevNodes = new List<NodeButton>();   // 위층(이전층) 노드들

    void Awake()
    {
        // 자식 중 Highlight 이름으로 찾기
        highlight = transform.Find("Highlight")?.gameObject;
        if (highlight != null)
            highlight.SetActive(false);

        // 자식 중 Icon 이미지 가져오기
        icon = transform.Find("Icon")?.GetComponent<Image>();
        if (icon == null)
            Debug.LogWarning("Icon Image가 없습니다!");
    }


    void Start()
    {
        RefreshVisual();
    }
   

    /// <summary>
    /// DungeonMaker.cs - GenerateDungeon()에서 생성된 기준대로 룸타입과 이미지 재위치.
    /// </summary>
    /// <param name="type">랜덤으로 정해진 RoomType수령</param>
    /// <param name="sprite">sprite 이미지 수령</param>
    public void SetRoomType(Enums.RoomType type, Sprite sprite)
    {
        //DungeonMaker로부터 수령한 타입 저장.
        CurrentRoomType = type;

        //수령한 타입이 None인 경우 isAvailable = false로 사용불가판정
        if (type == Enums.RoomType.None)
        {
            isAvailable = false;
            gameObject.SetActive(false);
            return;
        }

        //사용 가능시 세팅.
        isAvailable = true;
        gameObject.SetActive(true);

        if (icon != null)
        {
            icon.sprite = sprite;
            icon.color = Color.white;
        }

        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (icon == null) return;

        // 노드 활성화 여부(isAvailable) 에 따라서 알파값 변경
        var color = icon.color;
        color.a = isAvailable ? 1f : 0.3f; 
        icon.color = color;
    }


    // 마우스가 이미지 범위 위에 있음.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isAvailable && highlight != null)
            highlight.SetActive(true);
    }

    // 마우스가 이미지 범위 밖으로 나감.
    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlight != null)
            highlight.SetActive(false);
    }

}
