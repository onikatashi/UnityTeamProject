using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using static Enums; // RoomType Enum 접근

public class Portal : MonoBehaviour
{
    [Serializable]
    public struct RoomSettings
    {
        public Enums.RoomType roomType;
        [Tooltip("SceneNames 클래스에 정의된 변수명을 입력하세요.")]
        public string sceneFieldName;

        [Header("옵션 설정")]
        [Tooltip("체크 시 해당 노드를 클리어 처리합니다.")]
        public bool isClear;
        [Tooltip("체크 시 던전 데이터를 초기화하고 플레이어/인벤토리를 리셋합니다.")]
        public bool isReset;
    }

    [Header("방 타입별 상세 설정")]
    public RoomSettings[] roomSettingsList;

    private int playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogError("Portal: 'Player' 레이어가 존재하지 않습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 최상위 루트가 Player 레이어인지 확인
        if (other.transform.root.gameObject.layer == playerLayer)
        {
            if (SceneLoaderManager.Instance.GetCurrentSceneName() == SceneNames.Town)
            {
                for (int i = 0; i < Player.Instance.finalStats.startItemCount; i++)
                {
                    InventoryManager.Instance.AddItemToInventory(
                        ItemManager.Instance.GetRandomItemDataByRank(
                            ItemManager.Instance.GetRandomItemRank()));
                }
            }
            HandlePortalLogic();
        }
    }

    private void HandlePortalLogic()
    {
        DungeonManager dungeonManager = DungeonManager.Instance;
        if (dungeonManager == null) return;

        // 1. 현재 룸 타입 획득
        Enums.RoomType currentType = dungeonManager.GetCurrentRoomType();

        // 2. 인스펙터 설정값 찾아오기
        RoomSettings settings = GetSettingsForType(currentType);

        // 3. 조건별 로직 실행
        if (settings.isClear)
        {
            dungeonManager.dungeonClearSignal(); //
            Debug.Log($"Portal: {currentType} 노드 클리어 처리 완료.");
        }

        if (settings.isReset)
        {
            //// DungeonManager에 정의된 리셋 함수 호출
            //dungeonManager.OnStageCleared();

            if (currentType != Enums.RoomType.Boss)
            {
                return;
            }

            bool isFinalStage = dungeonManager.currentStage >= dungeonManager.maxStage;

            dungeonManager.needStageTransitionEffect = true;
            dungeonManager.OnStageCleared();
            dungeonManager.EnterNextStage();

            // 마지막 스테이지면 인스펙터에 적어둔 씬(Town)
            if (isFinalStage)
            {
                MoveToScene("Town");
            }
            // 아니면 다음 스테이지 맵
            else
            {
                MoveToScene("DungeonMap");
            }
            return;
        }

        // 4. 씬 이동 실행
        if (!string.IsNullOrEmpty(settings.sceneFieldName))
        {
            MoveToScene(settings.sceneFieldName);
        }
    }

    private RoomSettings GetSettingsForType(Enums.RoomType type)
    {
        foreach (var setting in roomSettingsList)
        {
            if (setting.roomType == type) return setting;
        }

        // 매핑 정보가 없을 경우 기본값 반환
        return new RoomSettings { sceneFieldName = "DungeonMap", isClear = false, isReset = false };
    }

    private void MoveToScene(string targetField)
    {
        var field = typeof(SceneNames).GetField(targetField);
        string actualSceneName = (field != null) ? field.GetValue(null).ToString() : targetField;

        // 포탈로 넘어가는 곳은 DungeonMap 밖에 없음 => 넘어갈 때, 룸타입 변경해줌으로써 BGM 재생
        DungeonManager.Instance.SetCurrentRoomType(RoomType.None);

        if (SceneLoaderManager.Instance != null)
        {
            SceneLoaderManager.Instance.LoadScene(actualSceneName); //
        }
        else
        {
            SceneManager.LoadScene(actualSceneName);
        }
    }
}