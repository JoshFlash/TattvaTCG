using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CombatAction { None, Attack, Defend, Cast }

[System.Serializable]
public struct CombatStats
{
    public int Block;
    public int Power;
    public int Health;

    public CombatStats(int block, int power, int health)
    {
        Block = block;
        Power = power;
        Health = health;
    }

    public static CombatStats operator +(CombatStats c, CombatStats d)
    {
        return new CombatStats(c.Block + d.Block, c.Power + d.Power, c.Health + d.Health);
    }
}
public abstract class Character : MonoBehaviour, ICardTarget
{
    [field: SerializeField] public TMP_Text BlockText { get; private set; }
    [field: SerializeField] public TMP_Text PowerText { get; private set; } 
    [field: SerializeField] public TMP_Text HealthText { get; private set; } 
    
    // The base stats of the Character as defined in their card
    [field: SerializeField] public CombatStats BaseStats { get; private set; } = new (1, 1, 10);
    [field: SerializeField] public bool IsFriendly { get; private set; } = false;

    [HideInInspector] public UnityEvent<CombatStats> OnStatsChanged = new ();
    
    // The current stat values as determined by base stats and any additional modifiers
    private CombatStats currentStats = default;

    public Lane Lane { get; set; } = default;
    public CombatAction SelectedAction { get; private set; } = CombatAction.None;
    public Character CombatTarget { get; private set; }

    private void LogStats()
    {
        Log.Info($"({name} stats) health: {currentStats.Health}, power: {currentStats.Power}, block: {currentStats.Block}");
    }

    private void Awake()
    {
        OnStatsChanged.AddListener(UpdateStatusText);
    }

    protected virtual void Start()
    {
        currentStats = new (0, BaseStats.Power, BaseStats.Health);
        
        OnStatsChanged.Invoke(currentStats);
    }

    private void OnDestroy()
    {
        OnStatsChanged.RemoveAllListeners();
    }

    private void UpdateStatusText(CombatStats stats)
    {
        PowerText.text = stats.Power.ToString();
        HealthText.text = stats.Health.ToString();
        if (stats.Block > 0)
        {
            HealthText.text += " + " + stats.Block;
        }
        
        BlockText.text = BaseStats.Block.ToString();
        
        LogStats();
    }

    public void SetStatsOnSummon(CombatStats statModifiers)
    {
        BaseStats += statModifiers;

        currentStats = BaseStats;
        ClearBlock();
    }

    public void ClearBlock()
    {
        currentStats.Block = 0;
        
        OnStatsChanged.Invoke(currentStats);
    }

    public void TakeDamage(int damage)
    {
        currentStats.Block -= damage;

        if (currentStats.Block < 0)
        {
            currentStats.Health += currentStats.Block;
            currentStats.Block = 0;
        }

        OnStatsChanged.Invoke(currentStats);
    }

    public void AddBlock(int block)
    {
        currentStats.Block += block;
        
        OnStatsChanged.Invoke(currentStats);
    }

    public void RestoreHealth(int healing)
    {
        currentStats.Health += healing;
        if (currentStats.Health > BaseStats.Health)
        {
            currentStats.Health = BaseStats.Health;
        }
        
        OnStatsChanged.Invoke(currentStats);
    }

    bool ICardTarget.IsFriendly()
    {
        return this.IsFriendly;
    }

    public void DisplayActions()
    {
        Log.NotImplemented("TODO - Implement Tooltips");
    }
    
    public async UniTask AssignAction(CombatAction combatAction)
    {
        Log.Info($"{name} will {combatAction} this turn");
        
        SelectedAction = combatAction;

        await UniTask.Yield();
    }

    public async UniTask ExecuteAssignedAction()
    {
        Log.NotImplemented("TODO - execute actions based on assigned combat action, target, and state of the play field");

        switch (SelectedAction)
        {
            case CombatAction.None:
                break;
            case CombatAction.Attack:
                // Todo - implement proper targeting
                var units = new List<Character>(GameObject.FindObjectsOfType<Character>())
                    .FindAll(character => !character.IsFriendly);
                CombatTarget = units[0];
                if (CombatTarget != null)
                {
                    CombatTarget.TakeDamage(currentStats.Power);
                }
                break;
            case CombatAction.Defend:
                AddBlock(BaseStats.Block);
                break;
            case CombatAction.Cast:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        await UniTask.Yield();
        
        SelectedAction = CombatAction.None;
    }
}