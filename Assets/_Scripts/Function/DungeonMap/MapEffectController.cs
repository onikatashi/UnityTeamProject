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

    // ★ 추가: 전환 전/후 테마 캐시 (연출용)
    private bool hasStageTransition = false;
    private Enums.DungeonTheme fromTheme;
    private Enums.DungeonTheme toTheme;

    void Awake()
    {
        Instance = this;

        if (DungeonManager.Instance != null &&
            DungeonManager.Instance.needStageTransitionEffect)
        {
            hasStageTransition = true;
            DungeonManager.Instance.needStageTransitionEffect = false;

            // ★ 연출용 캐시만
            fromTheme = DungeonManager.Instance.currentTheme;
            toTheme = DungeonManager.Instance.nextTheme;
        }
    }
        void Start()
    {
        //if (characterAnimator != null)
        //    characterAnimator.Play("Run");

        ResetBackgroundPositions();

        if (hasStageTransition)
        {
            // 1) 처음 화면은 이전 테마(fromTheme)
            InitAllBackgroundToTheme(fromTheme);

            // 2) 오른쪽(뒤) 배경만 다음 테마(toTheme)
            SetThemeToBackBackground(toTheme);

            // 3) 맵 올라오는 연출
            StartCoroutine(StageTransitionSequence());
        }
        else
        {
            // 일반 진입: 현재 테마로 통일
            InitAllBackgroundToTheme(DungeonManager.Instance.GetCurrentTheme());
            StartCoroutine(PlayMapUp());
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
        isBackgroundScrolling = false; // 내려가는 동안 멈춤(요구사항)

        Vector2 start = mapRoot.anchoredPosition;
        Vector2 end = start + Vector2.down * mapMoveDistance;

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
        Vector2 start = mapRoot.anchoredPosition;
        Vector2 end = Vector2.zero;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / mapMoveDuration;
            mapRoot.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        isBackgroundScrolling = true; // 올라온 뒤 재개
    }
}
