using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public float baseGoldMultiplier = 1f;
    public float bonusGoldMultiplier = 1f;

    PlayerGoldSystem goldSystem;
    Player player;

    private void Awake()
    {
        goldSystem = GetComponent<PlayerGoldSystem>();
        player = Player.Instance;
    }

    public void ReceiveRawGold(float rawGold)
    {
        float finalGold = CalculateFinalGold(rawGold);
        goldSystem.AddGold(finalGold);
    }

    float CalculateFinalGold(float rawGold)
    {
        float result = rawGold;

        result *= baseGoldMultiplier;
        result *= bonusGoldMultiplier;

        //플레이어 스탯 보너스도 적용
        result *= (1f + player.finalStats.bonusGoldRate);

        return result;
    }

}


