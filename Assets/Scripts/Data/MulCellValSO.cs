using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Mul Cell", menuName = "Scriptable Objects/Cell/Multiply")]
    public class MulCellValSO : ModifierCellSO
    {
        public int value;

        public override void Setup(Grid grid, Vector2Int pos)
        {
            base.Setup(grid, pos);
            // TODO
        }
    }
}
