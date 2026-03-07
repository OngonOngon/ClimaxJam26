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
            /* grid.AddModifierAt(ModifierType.Add, 1, new Vector2Int(0, 1));
            grid.AddModifierAt(ModifierType.Add, 2, new Vector2Int(1, 1));
            grid.AddNumberAt(2, new Vector2Int(0, 0));
            grid.AddNumberAt(1, new Vector2Int(1, 0));
            grid.AddNumberAt(1, new Vector2Int(2, 0)); */

            // Plus modifier 2
            /* grid.AddModifierAt(ModifierType.Add, 2, new Vector2Int(0, 1));
            grid.AddModifierAt(ModifierType.Add, 3, new Vector2Int(1, 1));
            grid.AddNumberAt(2, new Vector2Int(0, 0));
            grid.AddNumberAt(3, new Vector2Int(1, 0)); */

            // Plus modifier 3
            /* grid.AddModifierAt(ModifierType.Add, 2, new Vector2Int(1, 1));
            grid.AddNumberAt(1, new Vector2Int(1, 1));
            grid.AddNumberAt(1, new Vector2Int(1, 0));
            grid.AddNumberAt(2, new Vector2Int(2, 0)); */

            // Multiply modifier
            /* grid.AddModifierAt(ModifierType.Multiply, 9, new Vector2Int(1, 1));
            grid.AddModifierAt(ModifierType.Add, 2, new Vector2Int(2, 1));
            grid.AddNumberAt(9, new Vector2Int(1, 1)); */

            // Multiply add modifier
            /* grid.AddModifierAt(ModifierType.Multiply, 3, new Vector2Int(1, 2));
            grid.AddModifierAt(ModifierType.Multiply, 2, new Vector2Int(3, 3));
            grid.AddModifierAt(ModifierType.Add, 1, new Vector2Int(1, 1));
            grid.AddModifierAt(ModifierType.Add, 2, new Vector2Int(2, 1));
            grid.AddNumberAt(1, new Vector2Int(1, 0));
            grid.AddNumberAt(1, new Vector2Int(2, 0)); */

            // Tower
            grid.AddTowerAt(type: TowerType.Basic, letter: 'T', damage: 1, range: 4, hp: 3, aoe: 3, new Vector2Int(0, 0));
            grid.AddNumberAt(2, new Vector2Int(2, 2));
            grid.AddNumberAt(1, new Vector2Int(2, 1));
            grid.AddNumberAt(1, new Vector2Int(1, 2));
            grid.AddNumberAt(1, new Vector2Int(1, 1));
            grid.AddNumberAt(1, new Vector2Int(1, 3));
            grid.AddNumberAt(1, new Vector2Int(3, 1));
            grid.AddNumberAt(1, new Vector2Int(2, 3));
            grid.AddNumberAt(1, new Vector2Int(3, 2));
            grid.AddNumberAt(1, new Vector2Int(3, 3));
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
            else if (Input.GetKeyDown(KeyCode.S))
            {
                CommandSO cmd = ScriptableObject.CreateInstance<CommandSO>();
                cmd.changeType(CommandType.Shoot);
                grid.Command(cmd, new Vector2Int(0, 0));
            }
        }
    }
}
