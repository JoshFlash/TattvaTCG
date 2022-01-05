using System;
using UnityEditor;
using UnityEngine;

public class CardConfig : ScriptableObject
{
    private const string kCardConfigPath = "Cards/CardConfig";
    public static CardConfig Instance;
    
    public float MoveSpeed	= 0.21f;
    public float MinPadding	= 0.21f;
    public float MaxPadding	= 0.42f;
    public float DealtSpeed	= 0.21f;
    public float SortSpeed	= 0.14f;
		
    public float SelectHeight  = 0.42f;
    public float ExamineHeight = 0.5f;
    public float ExamineDepth  = -0.07f;
    public float DodgeDistance = 0.07f;
    public float DepthInterval = -0.01f;

    [InitializeOnLoadMethod]
    public static void Init()
    {
        Instance = Resources.Load<CardConfig>(kCardConfigPath);
    }
}