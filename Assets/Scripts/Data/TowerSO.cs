using UnityEngine;

namespace Dubinci
{
    public abstract class TowerSO : ScriptableObject
    {
        public char Letter;
        public int HP;
        public bool CanBeUsed;

        public virtual void Build(Grid grid, Vector2Int pos)
        {

        }

        public virtual void Activate(Grid grid, Vector2Int pos)
        {

        }
    }
}
