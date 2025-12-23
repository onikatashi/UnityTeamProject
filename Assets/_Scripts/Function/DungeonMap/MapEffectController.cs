using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapEffectController : MonoBehaviour
{
    public static MapEffectController Instance;

    //────────────────────────────────────
    // 캐릭터 연출
    //────────────────────────────────────
    [Header("Character")]
    public Transform characterRoot;
    public Animator characterAnimator;

    //────────────────────────────────────
    // 배경 스크롤 (UI 기반)
    //────────────────────────────────────
    [Header("Background Scroll")]
    public RectTransform backgroundRoot;     // 고정 루트
    public RectTransform[] backgrounds;       // 반드시 2개
    public float backgroundSpeed = 300f;
    public float backgroundWidth = 1920f;     // Reference Resolution 기준

    //────────────────────────────────────
    // 맵 전환 연출
    //────────────────────────────────────
    [Header("Map Transition")]
    public RectTransform mapRoot;
    public float mapMoveDistance = 900f;
    public float mapMoveDuration = 0.6f;

    //────────────────────────────────────
    // 외부 참조
    //────────────────────────────────────
    [Header("References")]
    public DungeonMaker dungeonMaker;

    private bool isBackgroundScrolling = true;

   
    void Start()
    {
        if (characterAnimator != null)
            characterAnimator.Play("Run");

        //RebindDungeonMaker();
        InitBackgroundPositions();
        InitBackgroundSprites();

        if (DungeonManager.Instance.needStageTransitionEffect)
        {
            DungeonManager.Instance.needStageTransitionEffect = false;
            StartCoroutine(StageTransitionSequence());
        }
        else
        {
            StartCoroutine(PlayMapUp()); // 일반 진입
        }

    }

    void Update()
    {
        if (isBackgroundScrolling)
            ScrollBackground();
    }

    private void InitBackgroundSprites()
    {
        if (dungeonMaker == null)
            return;

        Enums.DungeonTheme currentTheme = DungeonManager.Instance.GetCurrentTheme();
        Sprite sprite = dungeonMaker.GetThemeSprite(currentTheme);
        if (sprite == null)
            return;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            Image img = backgrounds[i].GetComponent<Image>();
            if (img == null) continue;

            img.sprite = sprite;
            img.color = Color.white;
        }

        Debug.Log("[MapEffectController] Background sprites initialized");
    }
    //────────────────────────────────────
    // DungeonMaker 재연결
    //────────────────────────────────────
    private void RebindDungeonMaker()
    {
        dungeonMaker = FindFirstObjectByType<DungeonMaker>();
    }

    //────────────────────────────────────
    // 배경 초기 위치 (중요)
    //────────────────────────────────────
    private void InitBackgroundPositions()
    {
        if (backgrounds == null || backgrounds.Length < 2)
            return;

        // 기준은 항상 화면 중앙
        backgrounds[0].anchoredPosition = Vector2.zero;
        backgrounds[1].anchoredPosition = new Vector2(backgroundWidth, 0f);

        Debug.Log("[MapEffectController] Background initialized");
    }

    //────────────────────────────────────
    // 배경 무한 스크롤 (anchoredPosition ONLY)
    //────────────────────────────────────
    private void ScrollBackground()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            RectTransform bg = backgrounds[i];

            bg.anchoredPosition += Vector2.left * backgroundSpeed * Time.deltaTime;

            if (bg.anchoredPosition.x <= -backgroundWidth)
            {
                bg.anchoredPosition += Vector2.right * backgroundWidth * backgrounds.Length;
            }
        }
    }

    //────────────────────────────────────
    // 노드 클릭 이동 연출
    //────────────────────────────────────
    public void PlayNodeMove()
    {
        StartCoroutine(NodeMoveSequence());
    }

    private IEnumerator NodeMoveSequence()
    {
        Vector3 start = characterRoot.position;
        Vector3 end = start + Vector3.right * 3.5f;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            characterRoot.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    //────────────────────────────────────
    // 스테이지 전환 연출
    //────────────────────────────────────
    public void PlayStageTransition()
    {
        StartCoroutine(StageTransitionSequence());
    }

    private IEnumerator StageTransitionSequence()
    {
        isBackgroundScrolling = false;
        // 1. 맵 내리기
        yield return PlayMapDown();


        DungeonManager.Instance.OnStageCleared();
        DungeonManager.Instance.EnterNextStage();

        // 4. 다음 테마 배경 준비
        PrepareNextStageBackground();

        // 5. 맵 올리기
        yield return PlayMapUp();

        isBackgroundScrolling = true;
    }

    //────────────────────────────────────
    // 맵 이동
    //────────────────────────────────────
    private IEnumerator PlayMapDown()
    {
        Debug.Log("[MapEffect] PlayMapDown START");
        Vector2 start = mapRoot.anchoredPosition;
        Vector2 end = start + Vector2.down * mapMoveDistance;

        float elapsed = 0f;

        while (elapsed < mapMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / mapMoveDuration;
            mapRoot.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
        Debug.Log("[MapEffect] PlayMapDown END");
    }

    private IEnumerator PlayMapUp()
    {
        Vector2 start = mapRoot.anchoredPosition;   // 현재(내려간 상태)
        Vector2 end = Vector2.zero;                 // 원래 위치

        float elapsed = 0f;

        while (elapsed < mapMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / mapMoveDuration;
            mapRoot.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        mapRoot.anchoredPosition = end;
    }

    //────────────────────────────────────
    // 다음 스테이지 배경 선적용 (핵심)
    //────────────────────────────────────
    private void PrepareNextStageBackground()
    {
        if (dungeonMaker == null)
            return;

        Enums.DungeonTheme theme = DungeonManager.Instance.GetCurrentTheme();
        Sprite sprite = dungeonMaker.GetThemeSprite(theme);
        if (sprite == null)
            return;

        RectTransform nextBg = GetRightSideBackground();
        Image img = nextBg.GetComponent<Image>();

        img.sprite = sprite;
        img.color = Color.white;

        Debug.Log("[MapEffectController] Next background prepared : " + theme);
    }

    //────────────────────────────────────
    // 오른쪽(화면 밖) 배경 찾기
    //────────────────────────────────────
    private RectTransform GetRightSideBackground()
    {
        RectTransform result = backgrounds[0];

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].anchoredPosition.x > result.anchoredPosition.x)
                result = backgrounds[i];
        }

        return result;
    }
}
