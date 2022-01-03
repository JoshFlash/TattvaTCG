using UnityEngine;

public class CardConfig : ScriptableObject
{
    private const string CARD_CONFIG_PATH = "Cards/CardConfig";

    private static CardConfig kGlobalSettings = null;
    public static CardConfig GlobalSettings => kGlobalSettings ? kGlobalSettings : (kGlobalSettings = Resources.Load<CardConfig>(CARD_CONFIG_PATH));
    
    public float MoveSpeed	= 0.21f;
    public float MinPadding	= 0.21f;
    public float MaxPadding	= 0.42f;
    public float DealtSpeed	= 0.21f;
    public float SortSpeed	= 0.14f;
		
    public float SelectHeight   = 0.42f;
    public float SelectDepth    = -0.02f;
    public float ExamineHeight  = 0.5f;
    public float ExamineDepth  = -0.14f;
    public float DodgeDistance  = 0.07f;
}