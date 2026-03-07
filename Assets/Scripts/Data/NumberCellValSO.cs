using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Number Cell", menuName = "Scriptable Objects/Cell/Number")]
    public class NumberCellValSO : CellValueSO
    {
        public int number;

        public override void Setup(Grid grid, Vector2Int pos)
        {
            grid.AddNumberAt(number, pos);
        }
    }
}
