using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Modif Cell", menuName = "Scriptable Objects/Cell/Modifier")]
    public class ModifierCellSO : CellValueSO
    {
        public ModifierType type = ModifierType.None;
        public string modifText;
        public int value;
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
            grid.AddModifierAt(type, value, pos);
        }
    }
}
