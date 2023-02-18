using System;
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
}
public abstract class Character : MonoBehaviour, ICardTarget
{
    [field: SerializeField] public TMP_Text BlockText { get; private set; }
    [field: SerializeField] public TMP_Text PowerText { get; private set; } 
    [field: SerializeField] public TMP_Text HealthText { get; private set; } 
    
    // The base stats of the Character as defined in their card
    [field: SerializeField] public int MaxHealth { get; private set; } = 10;
    [field: SerializeField] public int BasePower { get; private set; } = 1;
    [field: SerializeField] public int BaseBlock { get; private set; } = 1;
    [field: SerializeField] public bool IsFriendly { get; private set; } = false;

    [HideInInspector] public UnityEvent<CombatStats> OnStatsChanged = new ();
    [HideInInspector] public UnityEvent<int> OnPowerChanged = new ();
    [HideInInspector] public UnityEvent<int> OnBlockChanged = new ();
    
    // Tthe current stat values as determined by base stats and any additional modifiers
    private CombatStats stats = new CombatStats();

    public Lane Lane { get; set; } = default;
    public CombatAction SelectedAction { get; private set; } = CombatAction.None;
    public Character CombatTarget { get; private set; }

    private void LogStats()
    {
        Log.Info($"({name} stats) health: {stats.Health}, power: {stats.Power}, block: {stats.Block}");
    }
    
    protected void Start()
    {
        stats = new(BaseBlock, BasePower, MaxHealth);
        
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        PowerText.text = stats.Power.ToString();
        HealthText.text = stats.Health.ToString();
        BlockText.text = stats.Block.ToString();
        
        LogStats();
    }

    public void SetStatsOnSummon(int block, int power, int health)
    {
        MaxHealth = health;
        BasePower = power;
        BaseBlock = block;
        
        stats = new(0, power, health);

        UpdateStatusText();
    }

    public void TakeDamage(int damage)
    {
        stats.Block -= damage;

        if (stats.Block < 0)
        {
            stats.Health += stats.Block;
            stats.Block = 0;
        }

        OnStatsChanged.Invoke(stats);
        UpdateStatusText();
    }

    public void RestoreHealth(int healing)
    {
        stats.Health += healing;
        if (stats.Health > MaxHealth)
        {
            stats.Health = MaxHealth;
        }
        
        OnStatsChanged.Invoke(stats);
        UpdateStatusText();
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
                if (CombatTarget != null)
                {
                    CombatTarget.TakeDamage(stats.Power);
                }
                break;
            case CombatAction.Defend:
                stats.Block = BaseBlock;
                break;
            case CombatAction.Cast:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        await UniTask.Yield();
    }
}