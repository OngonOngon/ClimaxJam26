using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "VoidCellSO", menuName = "Scriptable Objects/Cell/Void")]
    public class VoidCellSO : CellValueSO
    {
        public string mainText = "~";
        public int number = -1;

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
