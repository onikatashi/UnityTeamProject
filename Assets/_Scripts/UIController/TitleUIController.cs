using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;
    public Button exitButton;

    SceneLoaderManager sceneLoaderManager;
    UIManager uiManager;

    void Start()
    {
        sceneLoaderManager = SceneLoaderManager.Instance;
        uiManager = UIManager.Instance;

        startButton.onClick.AddListener(startGame);
        settingButton.onClick.AddListener(settingButtonClick);
        exitButton.onClick.AddListener(QuitGame);
    }

    public void startGame()
    {
        sceneLoaderManager.LoadScene(SceneNames.Town);
    }

    public void settingButtonClick()
    {
        uiManager.settingUIController.ShowSettingPanel();
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }

}
