using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;             // 사운드 이름(키 값)
    public AudioClip clip;          // 사운드 파일

    [Range(0f, 1f)] 
    public float volume = 1f;       // 0 ~ 1 사이의 볼륨
    [Range(0f, 1f)]
    public float pitch = 1f;        // 피치 (고음 / 저음)

    public bool loop = false;       // 반복 재생 여부

    [HideInInspector]
    public AudioSource source;      // 사운드 재생기 컴포넌트
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // 인스펙터에서 설정할 사운드 목록
    [Header("BGM 사운드 목록")]
    public Sound[] bgmSounds;       // 배경 음악 목록

    [Header("Effect 사운드 목록")]
    public Sound[] effectSounds;    // 효과음 목록

    // 사운드 검색을 위한 딕셔너리
    Dictionary<string, Sound> soundDictionary;

    AudioSource bgmSource;
    AudioSource sfxSource;
    string currentBGM = "";

    [Header("볼륨 설정")]
    [Range(0f, 1f)]
    public float masterVolume = 0.5f;
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundDictionary = new Dictionary<string, Sound>();

        // BGM 재생 전용 AudioSource 생성
        GameObject bgmObject = new GameObject("BGM_Player");
        bgmObject.transform.SetParent(transform);
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.loop = true; // BGM 전용 플레이어는 반복 재생

        // SFX 재생 전용 AudioSource 생성
        GameObject sfxObject = new GameObject("SFX_Player");
        sfxObject.transform.SetParent(transform);
        sfxSource = sfxObject.AddComponent<AudioSource>();

        // 딕셔너리에 모든 사운드 데이터 등록
        // BGM과 SFX 모두 등록
        foreach (Sound s in bgmSounds) AddSoundToDictionary(s);
        foreach (Sound s in effectSounds) AddSoundToDictionary(s);

        // BGM 데이터 사전 로드 (Preload)
        if (bgmSounds != null && bgmSounds.Length > 0)
        {
            StartCoroutine(PreloadBGMs());
        }
    }

    // 딕셔너리에 사운드를 등록하는 헬퍼 함수
    private void AddSoundToDictionary(Sound s)
    {
        if (soundDictionary.ContainsKey(s.name))
        {
            Debug.LogWarning($"{s.name} 중복");
            return;
        }
        soundDictionary.Add(s.name, s);
    }

    IEnumerator PreloadBGMs()
    {
        foreach (Sound bgm in bgmSounds)
        {
            // AudioClip이 로드되었는지 확인
            if (bgm.clip.loadState != AudioDataLoadState.Loaded)
            {
                // 강제 로드 요청
                // 비동기로 진행됨
                bgm.clip.LoadAudioData();

                // 로드 완료될 때까지 대기
                while (bgm.clip.loadState == AudioDataLoadState.Loading)
                {
                    yield return null;
                }
            }
        }
    }

    // 효과음 재생
    public void Play(string name, float volumeScale = 1f)
    {
        // 딕셔너리에서 사운드 찾기
        if (!soundDictionary.TryGetValue(name, out Sound sound))
        {
            Debug.LogError($"Effect {name} 딕셔너리에 없음");
            return;
        }

        // 최종 볼륨 계산 및 설정 (마스터 * SFX * 사운드 * 추가배율)
        float finalVolume = masterVolume * sfxVolume * sound.volume * volumeScale;

        // 재생
        sfxSource.PlayOneShot(sound.clip, finalVolume);
    }


    // 배경 음악(BGM)을 재생합니다.
    public void PlayBGM(string name, float volumScale = 1f)
    {
        if (!soundDictionary.TryGetValue(name, out Sound bgm))
        {
            Debug.LogError($"BGM {name} 딕셔너리에 없음");
            return;
        }

        // 현재 재생 중인 BGM과 같으면 재생하지 않고 종료
        if (currentBGM == name && bgmSource.isPlaying)
        {
            Debug.Log($"BGM {name}이미 재생 중.");
            return;
        }

        // BGM 전용 AudioSource에 클립 설정
        bgmSource.clip = bgm.clip;

        // 최종 볼륨 설정 (마스터 * BGM * 사운드 * 추가배율)
        bgmSource.volume = masterVolume * bgmVolume * bgm.volume * volumScale;
        bgmSource.Play();

        currentBGM = name;
    }

    // BGM 정지
    public void StopBGM()
    {
        bgmSource.Stop();
        currentBGM = "";
    }

    // 마스터 볼륨 설정
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);

        // BGM 볼륨 즉시 갱신
        if (bgmSource.isPlaying && !string.IsNullOrEmpty(currentBGM))
        {
            Sound bgm = soundDictionary[currentBGM];
            bgmSource.volume = masterVolume * bgmVolume * bgm.volume;
        }
        // SFX 볼륨은 다음 재생 시 자동으로 적용됨
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);

        // BGM 볼륨 즉시 갱신
        if (bgmSource.isPlaying && !string.IsNullOrEmpty(currentBGM))
        {
            Sound bgm = soundDictionary[currentBGM];
            bgmSource.volume = masterVolume * bgmVolume * bgm.volume;
        }
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        // 효과음은 재생 시 볼륨이 적용되므로 별도로 갱신할 필요 없음
    }

    // BGM 일시 정지
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    // BGM 다시 재생
    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.UnPause();
        }
    }

    // 현재 BGM 클립 이름 반환
    public string GetCurrentBGMTitle()
    {
        return bgmSource.clip.name;
    }
}
