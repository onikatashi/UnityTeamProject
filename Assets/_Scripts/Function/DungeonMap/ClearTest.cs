using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearTest : MonoBehaviour
{

    // 버튼 OnClick()에 연결할 함수
    public void OnClearButton()
    {
        DungeonManager dm = DungeonManager.Instance;

        // 1. 방 클리어 처리 (공통)
        dm.dungeonClearSignal();

        // 2. 보스 방일 때만 스테이지 처리
        if (dm.GetCurrentRoomType() == Enums.RoomType.Boss)
        {
            dm.OnStageCleared();
        }

        // 3. 맵 씬으로 복귀
        SceneManager.LoadScene("02_DungeonMap");


        //// 1. 현재 노드 클리어 처리
        //DungeonManager.Instance.dungeonClearSignal();
        //DungeonManager.Instance.ResetDungeonData();

        //// 2. 맵 씬으로 복귀
        //SceneManager.LoadScene("02_DungeonMap");
    }
}
