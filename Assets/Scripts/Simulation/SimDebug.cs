using UnityEngine;

namespace Dubinci
{
    public class SimDebug : MonoBehaviour
    {
        public Vector2Int dim;
        private Grid grid;

        private void Awake()
        {
            grid = new Grid(dim);
            Debug.Log(dim);
            grid.AddNumberAt(5, new Vector2Int(1, 1));
        }
    }
}
