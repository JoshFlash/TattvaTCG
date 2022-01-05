using System;
using UnityEditor;
using UnityEngine;

public class CardConfig : ScriptableObject
{
    private const string kCardConfigPath = "Cards/CardConfig";
    
    [SerializeField] private float moveSpeed	= 0.21f;
    [SerializeField] private float minPadding	= 0.15f;
    [SerializeField] private float maxPadding	= 0.21f;
    [SerializeField] private float dealtSpeed	= 0.21f;
    [SerializeField] private float sortSpeed	= 0.14f;
    
    [SerializeField] private float selectHeight  = 0.42f;
    [SerializeField] private float examineHeight = 0.25f;
    [SerializeField] private float examineDepth  = -0.07f;
    [SerializeField] private float dodgeDistance = 0.07f;
    [SerializeField] private float depthInterval = -0.01f;

    public static float MoveSpeed { get; private set; }
    public static float MinPadding { get; private set; }
    public static float MaxPadding { get; private set; }
    public static float DealtSpeed { get; private set; }
    public static float SortSpeed { get; private set; }
    
    public static float SelectHeight { get; private set; }
    public static float ExamineHeight { get; private set; }
    public static float ExamineDepth { get; private set; }
    public static float DodgeDistance { get; private set; }
    public static float DepthInterval { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        var config = Resources.Load<CardConfig>(kCardConfigPath);
        
        MoveSpeed       = config.moveSpeed;
        MinPadding      = config.minPadding;
        MaxPadding      = config.maxPadding;
        DealtSpeed      = config.dealtSpeed;
        SortSpeed       = config.sortSpeed;
        SelectHeight    = config.selectHeight;
        ExamineHeight   = config.examineHeight;
        ExamineDepth    = config.examineDepth;
        DodgeDistance   = config.dodgeDistance;
        DepthInterval   = config.depthInterval;
    }
}