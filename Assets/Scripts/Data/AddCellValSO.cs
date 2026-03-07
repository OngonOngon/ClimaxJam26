using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Add Cell", menuName = "Scriptable Objects/Cell/Add")]
    public class AddCellValSO : ModifierCellSO
    {
        public int value;

        public override void Setup(Grid grid, Vector2Int pos)
        {
            base.Setup(grid, pos);
            // TODO
        }
    }
}
