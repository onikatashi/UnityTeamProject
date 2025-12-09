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
            Debug.LogWarning("SceneLoadManager: Player.Instance 없음.");
            return;
        }

        GameObject spawn = GameObject.Find("SpawnPoint");

        if (spawn == null)
        {
            Debug.LogError("SceneLoadManager: SpawnPoint 없음.");
            return;
        }

        Player.Instance.transform.position = spawn.transform.position;
    }
}
