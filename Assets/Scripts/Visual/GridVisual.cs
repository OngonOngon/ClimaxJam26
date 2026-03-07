using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dubinci
{
    public class GridVisual : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private CellVisual cellPrefab;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private CellValueSO emptyCellVal;

        [SerializeField, HideInInspector] private List<CellVisual> cells = new List<CellVisual>();

        private Grid grid;
        private Vector2Int selectedCell;

        [ContextMenu("Generate")]
        private void GenerateCells()
        {
            foreach (var cell in cells)
                DestroyImmediate(cell.gameObject);
            cells.Clear();

            gridLayout.constraintCount = gridSize.x;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    CellVisual cell = Instantiate(cellPrefab, gridLayout.transform);
                    cell.Create(this, new Vector2Int(x, y), emptyCellVal);
                    cells.Add(cell);
                }
            }
        }

        [ContextMenu("Regenerate")]
        private void RegenerateCells()
        {
            if (cells.Count != gridSize.x * gridSize.y)
            {
                Debug.LogError("Can't regenerate, sizes don't match. Use Generate instead or match prev size");
                return;
            }

            List<CellVisual> prevCels = new List<CellVisual>(cells);
            cells.Clear();

            gridLayout.constraintCount = gridSize.x;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    CellVisual cell = Instantiate(cellPrefab, gridLayout.transform);
                    cell.Create(prevCels[y + x * gridSize.y]);
                    cells.Add(cell);
                }
            }

            foreach (var cell in prevCels)
                DestroyImmediate(cell.gameObject);
        }

        private void Awake()
        {
            selectedCell = gridSize / 2;
            grid = new Grid(gridSize);
            foreach (var cell in cells)
                cell.Setup(grid);
            GetCell(selectedCell).HighliteCell();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grid.Tick();
                foreach (var cell in cells)
                    cell.UpdateVisual(grid.GetCell(cell.Pos));
            }
        }

        private CellVisual GetCell(Vector2Int pos)
        {
            return cells[pos.y + pos.x * gridSize.y];
        }

        public void MoveUp()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.y = (selectedCell.y + 1 + gridSize.y) % gridSize.y;
            Debug.Log(selectedCell);
            GetCell(selectedCell).HighliteCell();
        }

        public void MoveDown()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.y = (selectedCell.y - 1 + gridSize.y) % gridSize.y;
            Debug.Log(selectedCell);
            GetCell(selectedCell).HighliteCell();
        }

        public void MoveLeft()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.x = (selectedCell.x - 1 + gridSize.x) % gridSize.x;
            Debug.Log(selectedCell);
            GetCell(selectedCell).HighliteCell();
        }

        public void MoveRight()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.x = (selectedCell.x + 1 + gridSize.x) % gridSize.x;
            Debug.Log(selectedCell);
            GetCell(selectedCell).HighliteCell();
        }

        public void Select()
        {
            GetCell(selectedCell).SelectCell();
        }

        public void Unselect()
        {
            GetCell(selectedCell).HighliteCell();
        }
    }
}
