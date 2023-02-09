using UnityEngine;

[CreateAssetMenu(fileName = "New BattleData", menuName = "Configs/Battle Debug Data")]
public class BattleDebugData : ScriptableObject
{
    public GameObject PlayerChampionPrefab;
    public GameObject EnemyChampionPrefab;
    public GameObject PlayerDefaultCardInDeck;
    public int ChampionCardRotation = 160;
}