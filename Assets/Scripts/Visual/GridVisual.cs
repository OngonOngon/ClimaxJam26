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

        [ContextMenu("Generate")]
        private void GenerateCells()
        {
            foreach (var cell in cells)
                DestroyImmediate(cell.gameObject);
            cells.Clear();

            gridLayout.constraintCount = gridSize.x;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
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
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
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
            grid = new Grid(gridSize);
            foreach (var cell in cells)
                cell.Setup(grid);
        }
    }
}
