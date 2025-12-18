using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    // 씬 전환 (data 입력 시 데이터도 전달 가능)
    public void LoadScene(string sceneName, object data = null)
    {
        dataToPass = data;

        SceneManager.LoadScene(sceneName);
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
