using UnityEngine;

namespace Dubinci
{
    public class Grid
    {
        private Vector2Int dim;
        private Cell[,] cells;

        public Grid(Vector2Int dim)
        {
            this.dim = dim;

            cells = new Cell[dim.x, dim.y];
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    cells[x, y] = new Cell();
                }
            }
        }
        public void AddNumberAt(int number, Vector2Int pos)
        {
            if (!IsValidPos(pos))
            {
                return;
            }

            Cell targetCell = this.cells[pos.x, pos.y];

            targetCell.Content = new NumberEntity(number);
        }

        public bool IsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < dim.x && pos.y < dim.y;
        }
        public void Tick()
        {
            Debug.Log("Tickiiing!");
        }
        public void Command(CommandSO command, Vector2Int pos) { }
        public Cell GetCell(Vector2Int pos)
        {
            return cells[pos.x, pos.y];
        }
    }
}
