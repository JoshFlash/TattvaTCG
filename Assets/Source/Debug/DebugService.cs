using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugService : IGameService
{
    private const string kBattleDebugAssetPath = "BattleDebugData";
    public BattleDebugData BattleDebugData = default;
    
    private const string kDebugStarterDeck = "DebugStarterDeck";
    public StarterDeck DebugStarterDeck = default;

    public void Init()
    {
        BattleDebugData = Resources.Load<BattleDebugData>(kBattleDebugAssetPath);
        DebugStarterDeck = Resources.Load<StarterDeck>(kDebugStarterDeck);
    }

    public bool IsInitialized { get; set; }
}
