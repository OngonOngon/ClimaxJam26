using UnityEngine;

namespace Dubinci
{
    public class SimDebug : MonoBehaviour
    {
        public Vector2Int dim;
        private Grid grid;

        private void PrintGrid()
        {
            for (int x = 0; x < dim.x; x++)
            {
                string line = "";
                for (int y = 0; y < dim.y; y++)
                {
                    line += grid.GetCell(new Vector2Int(x, y)) + " ";
                }
                Debug.Log(line);
            }
        }

        private void Awake()
        {
            grid = new Grid(dim);
            Debug.Log(dim);
            grid.AddNumberAt(5, new Vector2Int(1, 1));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grid.Tick();
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                PrintGrid();
            }
        }
    }
}
