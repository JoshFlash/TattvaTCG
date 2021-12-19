using static UnityEngine.Random;

public static class RandomChance
{
    public static bool CoinFlip()
    {
        return Range(0, 2) < 1;
    }
}