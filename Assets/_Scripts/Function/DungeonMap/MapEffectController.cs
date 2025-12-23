using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapEffectController : MonoBehaviour
{
    public static MapEffectController Instance;

    [Header("Character")]
    public Transform characterRoot;
    public Animator characterAnimator;

    [Header("Background Scroll")]
    public RectTransform[] backgrounds; // 반드시 2개
    public float backgroundSpeed = 300f;
    public float backgroundWidth = 1920f;

    [Header("Map Transition")]
    public RectTransform mapRoot;
    public float mapMoveDistance = 900f;
    public float mapMoveDuration = 0.6f;

    [Header("References")]
    public DungeonMaker dungeonMaker;

    private bool isBackgroundScrolling = true;
    private bool isThemeSyncDone = false;

    private Vector2 mapBasePos;


    // ★ 추가: 전환 전/후 테마 캐시 (연출용)
    private bool hasStageTransition = false;
    private Enums.DungeonTheme fromTheme;
    private Enums.DungeonTheme toTheme;

    void Awake()
    {
        Instance = this;

        var dm = DungeonManager.Instance;
        if (dm != null && dm.needStageTransitionEffect)
        {
            dm.needStageTransitionEffect = false;

            // 1️ 연출용 캐시
            fromTheme = dm.currentTheme;
            toTheme = dm.nextTheme;
            hasStageTransition = true;

            // 2 여기서 게임 상태 확정
            dm.EnterNextStage();
        }
    }
    void Start()
    {
        mapBasePos = new Vector2(250f, 0f);  
        mapRoot.anchoredPosition = mapBasePos;

        ResetBackgroundPositions();

        if (hasStageTransition)
        {
            InitAllBackgroundToTheme(fromTheme);
            SetThemeToBackBackground(toTheme);
            StartCoroutine(StageTransitionSequence());
        }
        else
        {
            mapRoot.anchoredPosition = mapBasePos;

            InitAllBackgroundToTheme(DungeonManager.Instance.GetCurrentTheme());
        }
    }

    void Update()
    {
        if (isBackgroundScrolling)
            ScrollBackground();
    }

   

    //────────────────────────────────────
    // Stage Transition (연출만 담당)
    //────────────────────────────────────
    private IEnumerator StageTransitionSequence()
    {
        isThemeSyncDone = false;

        // 맵이 내려가서 초기화되는 느낌을 주고 싶으면 Down->Up 순서
        yield return PlayMapDown();

        // 여기서는 절대 OnStageCleared/EnterNextStage 호출하지 않는다 (Awake에서 이미 끝남)

        ResetBackgroundPositions();
        yield return PlayMapUp();
    }

    //────────────────────────────────────
    // Background Logic
    //────────────────────────────────────
    private void ScrollBackground()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            RectTransform bg = backgrounds[i];
            bg.anchoredPosition += Vector2.left * backgroundSpeed * Time.deltaTime;

            // ★ 경계 통과 순간(랩) = “현재 테마가 전부 지나간 순간”
            if (bg.anchoredPosition.x <= -backgroundWidth)
            {
                bg.anchoredPosition += Vector2.right * backgroundWidth * backgrounds.Length;

                // ★ 스테이지 전환 중이고, 아직 동기화 안했으면 여기서 1회 동기화
                if (hasStageTransition && !isThemeSyncDone)
                {
                    SyncAllBackgroundsToTheme(toTheme);
                    isThemeSyncDone = true;
                }
            }
        }
    }

    private void ResetBackgroundPositions()
    {
        backgrounds[0].anchoredPosition = Vector2.zero;
        backgrounds[1].anchoredPosition = new Vector2(backgroundWidth, 0f);
    }

    private void InitAllBackgroundToTheme(Enums.DungeonTheme theme)
    {
        Sprite sprite = dungeonMaker.GetThemeSprite(theme);
        if (sprite == null) return;

        foreach (var bg in backgrounds)
        {
            Image img = bg.GetComponent<Image>();
            img.sprite = sprite;
            img.color = Color.white;
        }
    }

    // 뒤 배경(오른쪽)만 지정 테마로
    private void SetThemeToBackBackground(Enums.DungeonTheme theme)
    {

        Sprite sprite = dungeonMaker.GetThemeSprite(theme);
        if (sprite == null) return;

        RectTransform back = GetRightSideBackground();
        Image img = back.GetComponent<Image>();
        img.sprite = sprite;
        img.color = Color.white;
    }

    private void SyncAllBackgroundsToTheme(Enums.DungeonTheme theme)
    {
        Sprite sprite = dungeonMaker.GetThemeSprite(theme);
        if (sprite == null) return;

        foreach (var bg in backgrounds)
        {
            Image img = bg.GetComponent<Image>();
            img.sprite = sprite;
            img.color = Color.white;
        }
    }

    private bool HasPassedThemeBoundary()
    {
        RectTransform left = GetLeftSideBackground();
        return left.anchoredPosition.x <= -backgroundWidth;
    }

    private RectTransform GetRightSideBackground()
    {
        return backgrounds[0].anchoredPosition.x > backgrounds[1].anchoredPosition.x
            ? backgrounds[0]
            : backgrounds[1];
    }

    private RectTransform GetLeftSideBackground()
    {
        return backgrounds[0].anchoredPosition.x < backgrounds[1].anchoredPosition.x
            ? backgrounds[0]
            : backgrounds[1];
    }

    //────────────────────────────────────
    // Map Move
    //────────────────────────────────────
    private IEnumerator PlayMapDown()
    {
        isBackgroundScrolling = false;

        Vector2 start = mapBasePos;
        Vector2 end = mapBasePos + Vector2.down * mapMoveDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / mapMoveDuration;
            mapRoot.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
    }


    private IEnumerator PlayMapUp()
    {
        Vector2 start = mapBasePos + Vector2.down * mapMoveDistance;
        Vector2 end = mapBasePos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / mapMoveDuration;
            mapRoot.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        isBackgroundScrolling = true;
    }
}
