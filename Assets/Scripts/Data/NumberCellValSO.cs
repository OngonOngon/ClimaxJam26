using UnityEngine;

namespace Dubinci
{
    public class NumberCellValSO : CellValueSO
    {
        public int number;

        public override void Setup(GridVisual grid, Vector2Int pos)
        {
            //grid.AddNumberAt(pos, number);
        }
    }
}
