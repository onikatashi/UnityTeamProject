using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 던전 방 타입 확률을 저장하는 SO
/// 2. 방 타입, 등장 확률 등을 포함
/// </summary>

    // 방(노드) 종류
    //public enum RoomType
    //{
    //    Normal,
    //    Elite,
    //    Shop,
    //    Rest,
    //    Forge,
    //    Boss
    //}

[CreateAssetMenu(fileName = "RoomTypeData", menuName = "Scriptable Objects/RoomTypeData")]
public class RoomTypeData : ScriptableObject
{


    [System.Serializable]
    public class RoomProbability
    {
        public Enums.RoomType roomType;

        [Range(0f, 100f)]
        public float probability;
    }

    public List<RoomProbability> roomList;



    //// Key: RoomType, Value: 등장 확률
    //public Dictionary<Enums.RoomType, float> roomTypeProbabilities = new Dictionary<Enums.RoomType, float>();


    //private void Awake()
    //{
    //    //딕셔너리 몬스터 던전 관련
    //    roomTypeProbabilities.Add(Enums.RoomType.Normal, 30f);
    //    roomTypeProbabilities.Add(Enums.RoomType.Elite, 20f);
    //    roomTypeProbabilities.Add(Enums.RoomType.Boss, 10f);

    //    roomTypeProbabilities.Add(Enums.RoomType.Shop, 10f);
    //    roomTypeProbabilities.Add(Enums.RoomType.Rest, 20f);
    //    roomTypeProbabilities.Add(Enums.RoomType.Forge, 10f);

    //}


    //public Enums.RoomType roomType;     // 방 타입별 등장 확률
    
    //public float RoomTypeProbabilities; // 해당 노드의 등장 확률
}