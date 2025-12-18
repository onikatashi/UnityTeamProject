using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public float totalFinalExpGained;

    public float baseExpMultiplier = 1f;        //기본 경험치 획득 배율 (1 = 100%)
    public float bonusExpMultiplier = 1f;       //보너스 경험치 획득 배율

    private PlayerLevelSystem levelSystem;

    private void Awake()
    {
        levelSystem = GetComponent<PlayerLevelSystem>();
    }

    public void ReceiveRawExp(float rawExp)
    {
        float finalExp = CalculateFinalExp(rawExp);
        AddFinalExp(finalExp);
    }

    float CalculateFinalExp(float rawExp)
    {
        float result = rawExp;

        result *= baseExpMultiplier;
        result *= bonusExpMultiplier;

        return result;
    }

    private void AddFinalExp(float finalExp)
    {
        totalFinalExpGained += finalExp;
        levelSystem.AddExp(finalExp);
    }
}
