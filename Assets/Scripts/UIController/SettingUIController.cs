using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    [Header("버튼 관련 UI")]
    public Button settingButton;                        // 세팅 버튼
    public GameObject settingPanel;                     // 세팅 패널 (온오프)
    public Button quitButton;                           // 설정창 닫기 버튼

    // 아직 활용 안함
    public Button exitButton;                           // 게임 중도포기, 게임 종료 등 이용할 버튼
    public TextMeshProUGUI exitButtonText;              // 게임 중도포기, 게임 종료 버튼 텍스

    [Header("사운드 관련 UI")]
    public Slider masterVolumeSlider;                   // 마스터 볼륨 슬라이더
    public Slider bgmVolumeSlider;                      // bgm 볼륨 슬라이더
    public Slider sfxVolumeSlider;                      // sfx 볼륨 슬라이더
    public TextMeshProUGUI masterValueText;             // 마스터 볼륨 퍼센트
    public TextMeshProUGUI bgmValueText;                // bgm 볼륨 퍼센트
    public TextMeshProUGUI sfxValueText;                // sfx 볼륨 퍼센트

    private const float DEBOUNCE_DELAY = 0.5f;          // 디바운싱 지연 시간 (볼륨 저장 지연시간)
    private Coroutine currentCoroutine;                 // 현재 진행 중인 저장 코루틴

    SoundManager soundManager;
    SaveLoadManager saveLoadManager;

    void Start()
    {
        soundManager = SoundManager.Instance;
        saveLoadManager = SaveLoadManager.Instance;

        // 버튼 연결
        settingButton.onClick.AddListener(ShowSettingPanel);
        quitButton.onClick.AddListener(HideSettingPanel);

        // 슬라이더 초기화
        masterVolumeSlider.value = soundManager.masterVolume;
        bgmVolumeSlider.value = soundManager.bgmVolume;
        sfxVolumeSlider.value = soundManager.sfxVolume;

        // 슬라이더 연결
        masterVolumeSlider.onValueChanged.AddListener(OnMasterChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXChanged);
    }


    void OnMasterChanged(float value)
    {
        soundManager.SetMasterVolume(value);
        saveLoadManager.settingData.masterVolume = value;
        UpdateVolumeText();

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 코루틴 시작
        currentCoroutine = StartCoroutine(DebounceSaveCoroutine());
    }

    void OnBGMChanged(float value)
    {
        soundManager.SetBGMVolume(value);
        saveLoadManager.settingData.bgmVolume = value;
        UpdateVolumeText();

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 코루틴 시작
        currentCoroutine = StartCoroutine(DebounceSaveCoroutine());
    }

    void OnSFXChanged(float value)
    {
        soundManager.SetSFXVolume(value);
        saveLoadManager.settingData.sfxVolume = value;
        UpdateVolumeText();

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 코루틴 시작
        currentCoroutine = StartCoroutine(DebounceSaveCoroutine());
    }

    void UpdateVolumeText()
    {
        masterValueText.text = $"{(masterVolumeSlider.value * 100):F0}%";
        bgmValueText.text = $"{(bgmVolumeSlider.value * 100):F0}%";
        sfxValueText.text = $"{(sfxVolumeSlider.value * 100):F0}%";
    }

    IEnumerator DebounceSaveCoroutine()
    {
        // 지연 시간 동안 대기
        yield return new WaitForSeconds(DEBOUNCE_DELAY);

        // 대기 시간 동안 취소되지 않았으면 저장
        saveLoadManager.SaveSettingData();

        currentCoroutine = null;
    }

    public void ShowSettingPanel()
    {
        if (settingPanel.activeSelf == false)
        {
            UpdateVolumeText();
            settingPanel.SetActive(true);
        }
        else
        {
            HideSettingPanel();
        }
    }

    public void HideSettingPanel()
    {
        settingPanel.SetActive(false);
    }
}
