using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Subtract Modifier", menuName = "Scriptable Objects/Modifier/Subtract")]
    public class SubtractModifierCellSO : ModifierCellSO
    {
        public override void Setup(Grid grid, Vector2Int pos)
        {
            grid.AddModifierAt(type, value, pos);
        }
    }
}
