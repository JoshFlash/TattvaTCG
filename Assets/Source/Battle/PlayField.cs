using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    [field: SerializeField] public Transform PlayerAnchor { get; private set; } = default;
    [field: SerializeField] public Transform OpponentAnchor { get; private set; } = default;
    
    [field: SerializeField] public Lane PlayerTopLane { get; private set; } = default;
    [field: SerializeField] public Lane PlayerBottomLane { get; private set; } = default;

    [field: SerializeField] public Lane OpponentTopLane { get; private set; } = default;
    [field: SerializeField] public Lane OpponentBottomLane { get; private set; } = default;

    public Champion PlayerChampion { get; set; } = default;
    public Champion OpponentChampion { get; set; } = default;
    
    public List<Character> GetAllFriendlyUnits()
    {
        var friendlyUnits = new List<Character>(PlayerTopLane.Minions);
        friendlyUnits.AddRange(PlayerBottomLane.Minions);
        friendlyUnits.Add(PlayerChampion);

        return friendlyUnits;
    }

    public List<Character> GetAllEnemyUnits()
    {
        var enemyUnits = new List<Character>(OpponentTopLane.Minions);
        enemyUnits.AddRange(OpponentBottomLane.Minions);
        enemyUnits.Add(OpponentChampion);

        return enemyUnits;
    }

    public List<Character> GetAllUnits()
    {
        return new List<Character>(GetAllFriendlyUnits().Concat(GetAllEnemyUnits()));
    }
}

