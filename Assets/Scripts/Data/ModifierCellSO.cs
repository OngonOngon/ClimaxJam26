using UnityEngine;

namespace Dubinci
{
    public abstract class ModifierCellSO : CellValueSO
    {
        public string modifText;
        public NumberCellValSO numberVal;

        public override void SetupUI(CellVisual vis)
        {
            base.SetupUI(vis);
            if (numberVal)
                numberVal.SetupUI(vis);
            vis.ShowModifier(modifText);
        }

        public override void Setup(Grid grid, Vector2Int pos)
        {
            if (numberVal)
                numberVal.Setup(grid, pos);
        }
    }
}
