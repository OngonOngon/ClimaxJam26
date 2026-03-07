using System.Text;
using UnityEngine;

namespace Dubinci
{
    public class SimDebug : MonoBehaviour
    {
        public Vector2Int dim;
        private Grid grid;

        private void PrintGrid()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = dim.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < dim.x; x++)
                {
                    Cell cell = grid.GetCell(new Vector2Int(x, y));
                    sb.Append(cell.GetContentString());
                    sb.Append(" ");
                }
                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }

        private void Awake()
        {
            grid = new Grid(dim);
            Debug.Log(dim);

            // Jonášovo grid
            /*
            grid.AddNumberAt(3, new Vector2Int(0, 0));
            grid.AddNumberAt(5, new Vector2Int(4, 0));
            grid.AddNumberAt(12, new Vector2Int(2, 2));
            */

            // Plus modifier
            grid.AddModifierAt(ModifierType.Add, 3, new Vector2Int(1, 1));
            grid.AddModifierAt(ModifierType.Add, 3, new Vector2Int(0, 1));
            grid.AddNumberAt(1, new Vector2Int(0, 0));
            grid.AddNumberAt(1, new Vector2Int(1, 0));
            grid.AddNumberAt(1, new Vector2Int(2, 0));
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
