using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearTest : MonoBehaviour
{

    // 버튼 OnClick()에 연결할 함수
    public void OnClearButton()
    {
        // 1. 현재 노드 클리어 처리
        DungeonManager.Instance.dungeonClearSignal();
        //DungeonManager.Instance.ClearDungeonData();

        // 2. 맵 씬으로 복귀
        SceneManager.LoadScene("02_DungeonMap");
    }
}
