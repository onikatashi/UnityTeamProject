using TMPro;
using UnityEngine;

public class PlayerGoldTextUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    PlayerGoldSystem goldSystem;

    private void Start()
    {
        //캐싱
        goldSystem = Player.Instance.GetComponent<PlayerGoldSystem>();
        
        //액션 넣어주기
        goldSystem.OnGoldChanged += UpdateGoldText;

        //업데이트 한번 시켜주기
        UpdateGoldText(goldSystem.currentGold);
    }

    private void OnDisable()
    {
        //액션 빼주기
        goldSystem.OnGoldChanged -= UpdateGoldText;
    }

    void UpdateGoldText(float amount)
    {
        goldText.text = amount.ToString(" Gold");
    }
}
