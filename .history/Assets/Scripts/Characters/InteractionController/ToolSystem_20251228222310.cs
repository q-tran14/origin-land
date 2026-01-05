using UnityEngine;

public class ToolSystem : MonoBehaviour
{
    public ToolType currentTool = ToolType.Hand;

    public int GetDamage()
    {
        return currentTool switch
        {
            ToolType.Axe => 2,
            ToolType.Pickaxe => 2,
            _ => 1
        };
    }

    public bool CanChop() => currentTool == ToolType.Axe;
    public bool CanMine() => currentTool == ToolType.Pickaxe;
}
