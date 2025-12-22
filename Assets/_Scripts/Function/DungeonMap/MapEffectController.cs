using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEffectController : MonoBehaviour
{
    public static MapEffectController Instance;

    //────────────────────────────────────
    // 캐릭터 연출
    //────────────────────────────────────
    [Header("Character")]
    public Transform characterRoot;          // 실제 이동 Transform
    public Animator characterAnimator;       // Run 애니메이션

    //────────────────────────────────────
    // 배경 스크롤 (Flappy Bird 방식)
    //────────────────────────────────────
    [Header("Background Scroll")]
    public Transform[] backgrounds;
    public float backgroundSpeed = 3f;
    public float backgroundWidth = 20f;

    //────────────────────────────────────
    // 맵 전환 연출
    //────────────────────────────────────
    [Header("Map Transition")]
    public Transform mapRoot;
    public float mapMoveDistance = 900f;
    public float mapMoveDuration = 0.6f;

    //────────────────────────────────────
    // 테마 전환
    //────────────────────────────────────
    [Header("Theme Transition")]
    public Image environmentBackground;
    public float themeFadeDuration = 0.4f;

    //────────────────────────────────────
    // 외부 참조
    //────────────────────────────────────
    [Header("References")]
    public DungeonMaker dungeonMaker;

    // 내부 상태
    private bool isBackgroundScrolling = true;
    private Action transitionFinishedCallback;


    //────────────────────────────────────
    // 초기 상태
    //────────────────────────────────────
    void Start()
    {
        if (characterAnimator != null)
            characterAnimator.Play("Run");
    }

    void Update()
    {
        if (isBackgroundScrolling)
            ScrollBackground();
    }

    //────────────────────────────────────
    // 씬 로드 시 DungeonMaker 재연결
    //────────────────────────────────────
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindDungeonMaker();
    }

    private void RebindDungeonMaker()
    {
        dungeonMaker = FindFirstObjectByType<DungeonMaker>();

        if (dungeonMaker == null)
            Debug.Log("[MapEffectController] DungeonMaker not found.");
        else
            Debug.Log("[MapEffectController] DungeonMaker rebound.");
    }

    //────────────────────────────────────
    // 배경 무한 스크롤
    //────────────────────────────────────
    private void ScrollBackground()
    {
        if (backgrounds == null) return;

        foreach (var bg in backgrounds)
        {
            if (bg == null) continue;

            bg.position += Vector3.left * backgroundSpeed * Time.deltaTime;

            if (bg.position.x <= -backgroundWidth)
            {
                bg.position += Vector3.right * backgroundWidth * backgrounds.Length;
            }
        }
    }

    //────────────────────────────────────
    // 노드 클릭 시 캐릭터 빠른 이동
    //────────────────────────────────────
    public void PlayNodeMove(Action onComplete)
    {
        StartCoroutine(NodeMoveSequence(onComplete));
    }

    private IEnumerator NodeMoveSequence(Action onComplete)
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

        onComplete?.Invoke();
    }

    //────────────────────────────────────
    // 스테이지 전환 연출
    // 맵 ↓ → 새 스테이지 → 테마 교체 → 맵 ↑
    //────────────────────────────────────
    public void PlayStageTransition(Action onFinished)
    {
        transitionFinishedCallback = onFinished;
        StartCoroutine(StageTransitionSequence());
    }

    private IEnumerator StageTransitionSequence()
    {
        // 1) 맵 내리기
        yield return PlayMapDown();

        // 2) 스테이지 갱신
        DungeonManager.Instance.EnterNextStage();

        // 3) 테마 교체 연출
        yield return ThemeChangeSequence();

        // 4) 맵 올리기
        yield return PlayMapUp();

        transitionFinishedCallback?.Invoke();
        transitionFinishedCallback = null;
    }

    private IEnumerator PlayMapDown()
    {
        Vector3 start = mapRoot.localPosition;
        Vector3 end = start + Vector3.down * mapMoveDistance;

        float elapsed = 0f;

        while (elapsed < mapMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / mapMoveDuration;
            mapRoot.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    private IEnumerator PlayMapUp()
    {
        Vector3 end = mapRoot.localPosition;
        Vector3 start = end + Vector3.down * mapMoveDistance;

        mapRoot.localPosition = start;

        float elapsed = 0f;

        while (elapsed < mapMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / mapMoveDuration;
            mapRoot.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    //────────────────────────────────────
    // 테마 교체 연출
    //────────────────────────────────────
    private IEnumerator ThemeChangeSequence()
    {
        if (environmentBackground == null || dungeonMaker == null)
            yield break;

        // Fade Out
        yield return FadeBackground(1f, 0f);

        // Sprite 교체
        ApplyCurrentThemeSprite();

        // Fade In
        yield return FadeBackground(0f, 1f);
    }

    private void ApplyCurrentThemeSprite()
    {
        var theme = DungeonManager.Instance.GetCurrentTheme();
        Sprite sprite = dungeonMaker.GetThemeSprite(theme);

        if (sprite != null)
        {
            environmentBackground.sprite = sprite;
            environmentBackground.color = Color.white;
        }
    }

    private IEnumerator FadeBackground(float from, float to)
    {
        float elapsed = 0f;
        Color color = environmentBackground.color;

        while (elapsed < themeFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / themeFadeDuration;

            color.a = Mathf.Lerp(from, to, t);
            environmentBackground.color = color;

            yield return null;
        }

        color.a = to;
        environmentBackground.color = color;
    }
}
