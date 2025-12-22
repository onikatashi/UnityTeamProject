using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 씬 이름을 상수로 선언하고 사용
// 씬 추가 할거면 여기에 상수명과 씬 이름 추가해주세요
public static class SceneNames
{
    public const string Title = "00_Title";
    public const string Town = "01_Town";
    public const string DungeonMap = "02_DungeonMap";
    public const string Dungeon = "03_Dungeon";
    public const string Restroom = "04_RestRoom";
}

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager Instance;

    // 씬 로드 시 전달할 데이터 (필요 시 이용)
    // 아마 매니저를 통해서 데이터를 대부분 얻어와서 안쓸 듯
    private object dataToPass = null;

    [Header("Loading UI")]
    public RectTransform mask;
    public Image blackLineGroup;                // 로딩시간이 짧은 씬 전환에는 검은줄을 이용한 간단한 로딩화면
    public GameObject progressGroup;            // 로딩시간이 긴 씬 전환에는 프로그레스 바를 이용한 로딩화면

    [Header("progress UI")]
    public Slider progressBar;                  // 프로그레스 바
    public TextMeshProUGUI progressValue;       // 로딩 진행도 텍스트

    public GameObject loadingImage;             // 로딩 화면에서 움직이는 이미지
    public Animation loadingAnimation;          // 로딩 애니메이션
    public float transitionDuration = 0.8f;     // 로딩 애니메이션 지속시간

    public float maxScale = 2500f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        HideAllUI();
    }

    private void HideAllUI()
    {
        blackLineGroup.gameObject.SetActive(false);
        progressGroup.SetActive(false);
    }

    // 씬 전환 (data 입력 시 데이터도 전달 가능)
    public void LoadScene(string sceneName, object data = null)
    {
        dataToPass = data;

        Enums.LoadingType type;
        if(sceneName == SceneNames.Title || sceneName == SceneNames.Dungeon || sceneName == SceneNames.Restroom)
        {
            type = Enums.LoadingType.ProgressBar;
        }
        else if(sceneName == SceneNames.Town || sceneName == SceneNames.DungeonMap)
        {
            type = Enums.LoadingType.BlackLines;
        }
        else
        {
            type = Enums.LoadingType.None;
            SceneManager.LoadScene(sceneName);
        }
        //SceneManager.LoadScene(sceneName);
        StartCoroutine(LoadSceneCoroutine(sceneName, type));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, Enums.LoadingType type)
    {
        HideAllUI();

        // 타입에 따라 연출 활성화
        switch (type)
        {
            case Enums.LoadingType.BlackLines:
                blackLineGroup.gameObject.SetActive(true);

                yield return StartCoroutine(AnimationIrisScale(maxScale, 0, transitionDuration));
                break;
            case Enums.LoadingType.ProgressBar:
                progressGroup.SetActive(true);
                break;
        }

        // 비동기 로딩 시작
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while(op.progress < 0.9f)
        {
            if (type == Enums.LoadingType.ProgressBar)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                progressBar.value = progress;
                progressValue.text = $"{progress * 100}:F0";
                // 돌아가는 이미지
                loadingAnimation.Play();
            }
            yield return null;
        }

        if(GetCurrentSceneName() == SceneNames.Title)
        {
            yield return StartCoroutine(SoundManager.Instance.PreloadBGMs());
        }

        // 씬 활성화
        op.allowSceneActivation = true;

        // 검은줄 연출 마무리
        yield return StartCoroutine(AnimationIrisScale(0, maxScale, transitionDuration));

        HideAllUI();
    }

    IEnumerator AnimationIrisScale(float start, float end, float duration)
    {
        float elapsed = 0f;

        Vector3 startScale = new Vector3(start, start, 1);
        Vector3 endScale = new Vector3(end, end, 1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mask.sizeDelta = Vector3.Lerp(startScale, endScale, elapsed / duration);
            yield return null;
        }
        mask.sizeDelta = endScale;
    }

    // 전달한 데이터 얻어오기
    public object GetData()
    {
        object data = dataToPass;
        // 데이터 를 사용했으면 초기화 (다음 씬으로 데이터 넘어가는 것 방지)
        dataToPass = null;
        return data;
    }

    // 현재 씬 이름 리턴
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // 씬 리로드는 사용할 일이 없을 것 같음
    //public void ReoladCurrentScene()
    //{
    //    string currentSceneName = SceneManager.GetActiveScene().name;
    //    LoadScene(currentSceneName);
    //}
}
