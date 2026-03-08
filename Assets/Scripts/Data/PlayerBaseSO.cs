using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "PlayerBase", menuName = "Scriptable Objects/Cell/PlayerBase")]
    public class PlayerBaseSO : CellValueSO
    {

        public override void SetupUI(CellVisual vis)
        {
            base.SetupUI(vis);
            vis.SetMainText("$");
        }

        public override void Setup(Grid grid, Vector2Int pos)
        {
            grid.AddPlayerBase(pos);
        }
    }
}
