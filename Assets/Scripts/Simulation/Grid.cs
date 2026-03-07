using UnityEngine;

namespace Dubinci
{
    public class Grid
    {
        private Cell[,] cells;

        public Grid(Vector2Int dim)
        {
            cells = new Cell[dim.x, dim.y];
        }
        public void AddNumberAt(int number, Vector2Int pos) { }
        public void Tick() { }
        public void Command(CommandSO command, Vector2Int pos) { }
        public Cell GetCell(Vector2Int pos)
        {
            return cells[pos.x, pos.y];
        }
    }
}
