using UnityEngine;

namespace Dubinci
{
    public class TowerSO : ScriptableObject
    {
        public char Letter;
        public int HP;

        public virtual void Build(Grid grid, Vector2Int pos)
        {

        }

        public virtual void Activate(Grid grid, Vector2Int pos)
        {

        }
    }
}
