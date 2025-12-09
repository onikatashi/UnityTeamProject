using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 방 타입, 등장 확률 등을 포함
/// 확률형 Room종류 : Normal(일반),  Elite(엘리트), Shop(상점), Rest(휴식), Forge(대장간), 
/// 예          외 : Boss(보스룸), Start - 시작과 끝 고정
/// </summary>

public class RoomTypeData : MonoBehaviour
{
    // 1.기본 방 확률 딕셔너리. <방종류, 확률>
    private Dictionary<Enums.RoomType, float> roomProbabilities
        = new Dictionary<Enums.RoomType, float>();

    // 2.설정 층 타입 강제설정 딕셔너리. <층 수, 설정할 방 종류>
    private Dictionary<int, Enums.RoomType> forcedFloors
        = new Dictionary<int, Enums.RoomType>();

    // 3.설정 노드 타입 강제설정 딕셔너리. <(행, 열), 설정할 방 종류>
    private Dictionary<(int floor, int column), Enums.RoomType> forcedRooms
        = new Dictionary<(int, int), Enums.RoomType>();

    //-----------------------------------------------------------------------

    void Awake()
    {
        InitProbabilities();    //초기화 시 Room 기본확률 확률 설정
        InitForcedRooms();      //특정 (층 or 노드) 강제 Type 설정.
    }

    //-----------------------------------------------------------------------
    private void InitProbabilities()
    {

        //딕셔너리를 통한 기본 확률 설정
        roomProbabilities.Clear();

        roomProbabilities.Add(Enums.RoomType.Normal,30f);
        roomProbabilities.Add(Enums.RoomType.Elite, 20f);
        roomProbabilities.Add(Enums.RoomType.Shop,  10f);
        roomProbabilities.Add(Enums.RoomType.Rest,  10f);
        roomProbabilities.Add(Enums.RoomType.Forge, 10f);
        roomProbabilities.Add(Enums.RoomType.None,  20f);
    }


    // 강제 노드 설정.
    // - 보스방 직전 휴식 or 상점 강제 설정
    // - 테스트시 1열 강제 설정 등.
    private void InitForcedRooms()
    {
        // [4층 2열] → 휴식방
        // 예시 : forcedRooms.Add((4, 2), Enums.RoomType.Rest);

        // [7층은 전부 상점]
        // 예시forcedFloors.Add(7, Enums.RoomType.Shop);
    }

    //-----------------------------------------------------------------------


    //DungeonMaker.cs에서 노드 생성시 호출.
    public Enums.RoomType GetRoomType(int floor, int column)
    {
        // 1️. 특정 노드 강제
        if (forcedRooms.TryGetValue((floor, column), out var nodeType))
            return nodeType;

        // 2️. 층 전체 강제
        if (forcedFloors.TryGetValue(floor, out var floorType))
            return floorType;

        // 3️. 기본 확률 기반 랜덤
        return GetRandomByProbability();
    }


    //GetRandomByProbability
    //확률적 방 추천 로직
    //1. 모든 RoomType의 확률을 모두 더함.   (예시 : Normal 30 + Elite 20 + Shop 10 == 60)
    //2. 0 ~ 전체 확률 사이의 랜덤값을 생성.  (예시 : 0~60중 randRoomChoice == 54라면)

    //3. 현재값(currentValue)보다 작을 때 까지 순환 누적하여 True시 해당 키값 반환.
    //[예시]
    //if (randRoomChoice <= currentValue)
    //  54 <= 30 (Normal 30%  => False)
    //  54 <= 50 (Normal 30% + Elite 20% == 50% =>  False)
    //  54 <= 60 (Normal 30% + Elite 20% + Shop 10% == 60% => True) Shop 반환.
    //

    private Enums.RoomType GetRandomByProbability()
    {
        float total = 0f;

        foreach (var probability in roomProbabilities)// total에 현재 확률 누적
        {
            total += probability.Value; 
        }
        
        float randRoomChoice = Random.Range(0, total);  //랜덤 방 추첨(0~total 중 하나 뽑기)
        
        float currentValue = 0f;
        foreach (var probability in roomProbabilities)
        {
            currentValue += probability.Value;

            if (randRoomChoice <= currentValue)
            {
                return probability.Key; //현재 구간에 해당하는 방 타입 반환 (반환 예시: Enums.RoomType.XXX)
            }
        }
        return Enums.RoomType.Normal; // 예외 처리.
    }


}