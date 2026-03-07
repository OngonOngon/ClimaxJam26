using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Empty Cell", menuName = "Scriptable Objects/Cell/Empty")]
    public class CellValueSO : ScriptableObject
    {
        public string text;

        public virtual void Setup(Grid grid, Vector2Int pos)
        {

        }
    }
}
