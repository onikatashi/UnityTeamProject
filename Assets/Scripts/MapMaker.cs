using UnityEngine;
using static Enums;

// DungeonManager의 테마 적용 로직을 분리한 스크립트
public class MapMaker : MonoBehaviour
{
    [Header("테마 그룹들 (씬에서 직접 드래그)")]
    public GameObject grassGroup;
    public GameObject snowGroup;
    public GameObject desertGroup;
    public GameObject lavaGroup;

    [Header("DungeonManager 참조 실패 시 기본 테마")]
    // 인스펙터에서 기본 테마를 설정할 수 있게 합니다.
    public DungeonTheme defaultThemeOnFailure = DungeonTheme.Grass;

    private void Start()
    {
        // DungeonManager의 싱글톤 인스턴스를 사용하여 테마를 가져옵니다.
        ApplyThemeFromManagerOrDefault();
    }

    /// <summary>
    /// DungeonManager에서 현재 테마를 받아와 적용하거나, 실패 시 기본 테마를 적용합니다.
    /// </summary>
    public void ApplyThemeFromManagerOrDefault()
    {
        DungeonTheme themeToApply = DungeonTheme.None;

        // DungeonManager는 싱글톤 패턴이므로 Instance를 통해 직접 접근합니다.
        DungeonManager manager = DungeonManager.Instance;

        // 1. DungeonManager.Instance가 유효한지 확인
        if (manager != null)
        {
            // GetCurrentTheme() 메서드 호출
            themeToApply = manager.GetCurrentTheme();

            if (themeToApply != DungeonTheme.None)
            {
                Debug.Log($"MapMaker: DungeonManager에서 테마 **{themeToApply}**를 성공적으로 가져왔습니다.");
            }
            else
            {
                Debug.LogWarning("MapMaker: DungeonManager는 존재하지만, currentTheme이 None입니다. (테마 초기화 오류 가능성). 기본 테마로 대체합니다.");
            }
        }
        else
        {
            // 2. DungeonManager 참조 실패 (Instance가 null인 경우)
            Debug.LogError("MapMaker: DungeonManager.Instance를 찾을 수 없습니다. 인스펙터에서 지정된 기본 테마로 대체합니다.");
        }

        // 3. DungeonManager에서 유효한 테마를 얻지 못한 경우 기본 테마 사용
        if (themeToApply == DungeonTheme.None)
        {
            themeToApply = defaultThemeOnFailure;
            Debug.Log($"MapMaker: 기본 테마 **{themeToApply}**를 적용합니다.");
        }

        // 4. 최종적으로 테마 적용
        ApplyTheme(themeToApply);
    }

    /// <summary>
    /// 실제 테마 적용 로직
    /// </summary>
    private void ApplyTheme(DungeonTheme theme)
    {
        DisableAllThemes();

        switch (theme)
        {
            case DungeonTheme.Grass:
                if (grassGroup) grassGroup.SetActive(true);
                break;

            case DungeonTheme.Snow:
                if (snowGroup) snowGroup.SetActive(true);
                break;

            case DungeonTheme.Desert:
                if (desertGroup) desertGroup.SetActive(true);
                break;

            case DungeonTheme.Lava:
                if (lavaGroup) lavaGroup.SetActive(true);
                break;

            case DungeonTheme.None:
                // 이 경고는 거의 발생하지 않아야 합니다.
                Debug.LogWarning("Theme.None이 전달되었습니다. 어떤 테마도 적용되지 않았습니다.");
                break;
        }

        Debug.Log("MapMaker 테마 적용 완료: " + theme);
    }

    /// <summary>
    /// 모든 테마 그룹 비활성화
    /// </summary>
    private void DisableAllThemes()
    {
        if (grassGroup) grassGroup.SetActive(false);
        if (snowGroup) snowGroup.SetActive(false);
        if (desertGroup) desertGroup.SetActive(false);
        if (lavaGroup) lavaGroup.SetActive(false);
    }
}