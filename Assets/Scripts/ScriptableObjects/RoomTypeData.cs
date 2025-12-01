using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 던전 방 타입 확률을 저장하는 SO
/// 2. 방 타입, 등장 확률 등을 포함
/// </summary>
[CreateAssetMenu(fileName = "RoomTypeData", menuName = "Scriptable Objects/RoomTypeData")]
public class RoomTypeData : ScriptableObject
{
    public Dictionary<Enums.RoomType, float> roomTypeProbabilities;     // 방 타입별 등장 확률
}
