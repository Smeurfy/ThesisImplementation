using System;
using UnityEngine;

public class NoveltyAndPerformanceFunctions : MonoBehaviour
{
    private const int minIntendedPerformance = 15;
    private const int maxIntendedPerformance = 90;

    private static int noveltyForRoom = 80;

    internal float CalculateIntendedPerformanceValueForRoom(int roomID)
    {
        //return Mathf.Clamp( (float) ((Mathf.Cos( roomID / 2 ) - 0.03 * roomID) + 3 ) * 25, minIntendedPerformance, maxIntendedPerformance);
        return (-roomID * 3.1f) + 90;
    }

    internal float CalculateIntendedNoveltyValueForRoom(int roomID)
    {
         return noveltyForRoom;   // enemies should not vary
        // return 100; // enemies should always be different
        // return 50;  // enemies should vary somewhat, but not that much
        
    }

    public void SetNoveltyValueByPlayersInput(int newNoveltyForRoom)
    {
        noveltyForRoom = newNoveltyForRoom;
    }
}
