using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string targetSceneName;

    private int playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogError("Portal: 'Player' 레이어가 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;

        if (root.gameObject.layer == playerLayer)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("Portal: targetSceneName이 비어있습니다.");
            }
        }
    }
}
