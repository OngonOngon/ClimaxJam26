using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Empty Cell", menuName = "Scriptable Objects/Cell/Empty")]
    public class CellValueSO : ScriptableObject
    {
        public virtual void SetupUI(CellVisual vis)
        {
            vis.ClearViz();
        }

        public virtual void Setup(Grid grid, Vector2Int pos)
        {
        }
    }
}
