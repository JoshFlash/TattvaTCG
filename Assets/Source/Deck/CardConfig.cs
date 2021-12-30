using UnityEngine;

public class CardConfig : ScriptableObject
{
    public static CardConfig GlobalSettings = new();
    
    public float MoveSpeed	= 0.21f;
    public float MinPadding	= 0.21f;
    public float MaxPadding	= 0.42f;
    public float DealtSpeed	= 0.21f;
    public float SortSpeed	= 0.14f;
		
    public Vector2 CardAreaPadding = new (0.14f, 0.28f);
    public Vector2 MouseOverBounds = new (0.07f, 0.21f);

    public float SelectHeight   = 0.42f;
    public float SelectDepth    = -0.02f;
    public float ExamineHeight  = 0.21f;
    public float DodgeDistance  = 0.07f;

    public float ExamineDodgeDistance = 0.04f;
    public float DodgeRightDistance = 0.04f;
}