using Cysharp.Threading.Tasks;
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

    public async UniTask UpdateUnitActions()
    {
        CheckMouseOverUnit(out mouseOverUnit);
        if (mouseOverUnit != null)
        {
            mouseOverUnit.DisplayActions();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            var results = MainCamera.ScreenCast(unitLayer);
            foreach (var result in results)
            {
                if (result.collider.TryGetComponent(out ActionButton actionButton))
                {
                    await actionButton.character.AssignAction(actionButton.CombatAction);
                }
            }
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
