using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugService : IGameService
{
    private const string kBattleDebugAssetPath = "BattleDebugData";
    public BattleDebugData BattleDebugData = default;

    public void Init()
    {
        BattleDebugData = Resources.Load<BattleDebugData>(kBattleDebugAssetPath);
    }

    public bool IsInitialized { get; set; }
}
