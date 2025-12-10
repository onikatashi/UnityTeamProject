using UnityEngine;

/// <summary>
/// 1. 방 종류 확률에 따라 맵 생성
/// </summary>
public class DungeonMaker : MonoBehaviour
{
    public int maxFloor = 7;   // 총 층 수
    public int maxColumn = 5;  // 1층당 노드 개수

    public RoomTypeData roomTypeData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Enums.RoomType[,] dungeonMap;

    void Start()
    {
        dungeonMap = new Enums.RoomType[maxFloor, maxColumn];

        GenerateDungeon();
        PrintDungeonToConsole();
    }

    private void GenerateDungeon()
    {
        for (int floor = 0; floor < maxFloor; floor++)
        {
            for (int col = 0; col < maxColumn; col++)
            {
                Enums.RoomType type = roomTypeData.GetRoomType(floor, col);
                dungeonMap[floor, col] = type;
            }
        }
    }

    private void PrintDungeonToConsole()
    {
        Debug.Log("===== Dungeon Result =====");

        for (int floor = 0; floor < maxFloor; floor++)
        {
            string line = $"Floor {floor}: ";

            for (int col = 0; col < maxColumn; col++)
            {
                line += dungeonMap[floor, col].ToString().PadRight(8);
            }

            Debug.Log(line);
        }
    }
    
}
