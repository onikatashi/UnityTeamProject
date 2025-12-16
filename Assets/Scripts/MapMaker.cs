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

    /// <summary>
    /// DungeonManager에서 호출되어 테마를 적용합니다.
    /// </summary>
    public void ApplyTheme(DungeonTheme theme)
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
                Debug.LogWarning("Theme.None이 전달되었습니다. 기본 테마가 적용되지 않았습니다.");
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