using UnityEngine;

[System.Serializable]
public class UserData
{

    public int level;
    public float currentExp;
    public float maxExp;

    // 특성 포인트 및 특성 목록 추후에 추가

    public UserData()
    {
        level = 1;
        currentExp = 0;
        maxExp = 100;
    }
}
