using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Number Cell", menuName = "Scriptable Objects/Cell/Number")]
    public class NumberCellValSO : CellValueSO
    {
        public string mainText;
        public int number;

        public override void SetupUI(CellVisual vis)
        {
            base.SetupUI(vis);
            vis.SetMainText(mainText);
        }

        public override void Setup(Grid grid, Vector2Int pos)
        {
            grid.AddNumberAt(number, pos);
        }
    }
}
    