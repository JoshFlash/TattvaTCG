using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New CardMovementConfig", menuName = "Configs/Card Movement Config")]
public class CardMovementConfig : ScriptableObject
{
    private const string kCardConfigPath = "CardMovementConfig";
    
    [SerializeField] private float moveSpeed	= 0.21f;
    [SerializeField] private float minPadding	= 1.5f;
    [SerializeField] private float maxPadding	= 2.1f;
    [SerializeField] private float dealtSpeed	= 0.21f;
    [SerializeField] private float sortSpeed	= 0.14f;
    [SerializeField] private float summonSpeed	= 0.21f;
    
    [SerializeField] private float selectHeight  = 6.4f;
    [SerializeField] private float selectDepth  = -4.2f;
    [SerializeField] private float examineHeight = 2.5f;
    [SerializeField] private float examineDepth  = -0.7f;
    [SerializeField] private float dodgeDistance = 0.7f;
    [SerializeField] private float depthInterval = -0.16f;
    [SerializeField] private float swapTolerance = 0.07f;
    
    [SerializeField] private float highlightDepth = -2.1f;
    [SerializeField] private float highlightHeight = -0.7f;

    public static float MoveSpeed { get; private set; }
    public static float MinPadding { get; private set; }
    public static float MaxPadding { get; private set; }
    public static float DealtSpeed { get; private set; }
    public static float SortSpeed { get; private set; }
    public static float SummonSpeed { get; private set; }
    
    public static float SelectHeight { get; private set; }
    public static float SelectDepth { get; private set; }
    public static float ExamineHeight { get; private set; }
    public static float ExamineDepth { get; private set; }
    public static float DodgeDistance { get; private set; }
    public static float DepthInterval { get; private set; }
    
    public static float SwapTolerance { get; private set; }
    public static float HighlightDepth { get; private set; }
    public static float HighlightHeight { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        var config = Resources.Load<CardMovementConfig>(kCardConfigPath);
        
        MoveSpeed       = config.moveSpeed;
        MinPadding      = config.minPadding;
        MaxPadding      = config.maxPadding;
        DealtSpeed      = config.dealtSpeed;
        SortSpeed       = config.sortSpeed;
        SelectHeight    = config.selectHeight;
        SelectDepth     = config.selectDepth;
        ExamineHeight   = config.examineHeight;
        ExamineDepth    = config.examineDepth;
        DodgeDistance   = config.dodgeDistance;
        DepthInterval   = config.depthInterval;
        SwapTolerance   = config.swapTolerance;
        SummonSpeed     = config.summonSpeed;
        HighlightDepth  = config.highlightDepth;
        HighlightHeight = config.highlightHeight;
    }
}