using UnityEngine;
using UnityEngine.UI;

namespace Dubinci
{
    public class GridVisual : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private CellVisual cellPrefab;
        [SerializeField] private GridLayoutGroup gridLayout;

        private void Awake()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {

                }
            }
        }
    }
}
