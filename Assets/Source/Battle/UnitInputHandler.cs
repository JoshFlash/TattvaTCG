using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInputHandler
{
    public UnitInputHandler()
    {
        unitLayer = LayerMask.GetMask("Units");
    }
    
    private readonly int unitLayer = default;

    private Character mouseOverUnit = default;
    
    private bool abeyInput = false;
    public bool IsReceivingInput => !abeyInput;

    public void UpdateMouseOverUnit()
    {
        CheckMouseOverUnit(out mouseOverUnit);
        if (mouseOverUnit != null)
        {
            mouseOverUnit.DisplayActions();
        }
    }

    private void CheckMouseOverUnit(out Character character)
    {
        character = null;
        var results = MainCamera.ScreenCast(unitLayer);
        foreach (var result in results)
        {
            if (result.collider.TryGetComponent(out character))
            {
                break;
            }
        }
    }
    
}
