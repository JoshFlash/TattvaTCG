using System.Collections.Generic;
using static UnityEngine.Random;

public static class Probability
{
    public static bool CoinFlip()
    {
        return Range(0, 2) < 1;
    }

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}