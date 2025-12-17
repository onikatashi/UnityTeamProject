using UnityEngine;

public class SceneLoadManager : MonoBehaviour
{
    private void Start()
    {
        MovePlayerToSpawn();
    }

    private void MovePlayerToSpawn()
    {
        if (Player.Instance == null)
        {
            Debug.LogWarning("SceneLoadManager: Player.Instance가 존재하지 않습니다.");
            return;
        }

        GameObject spawn = GameObject.Find("SpawnPoint");

        if (spawn == null)
        {
            Debug.LogError("SceneLoadManager: SpawnPoint를 찾을 수 없습니다.");
            return;
        }

        Player.Instance.transform.position = spawn.transform.position;
    }
}
