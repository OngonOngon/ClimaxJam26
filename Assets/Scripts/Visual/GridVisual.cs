using DG.Tweening;
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
        [SerializeField] private List<BuildCommandSO> buildCommands;
        [SerializeField] private List<BuildModifierSO> buildModifierCommands;
        [SerializeField] private CommandSO shootCommand;
        [SerializeField] private CommandSO shootAllCommand;
        [SerializeField] private CommandSO upgradeCommand;
        [SerializeField] private GameObject hintL;
        [SerializeField] private GameObject hintR;

        // Reference to the Game Over / Level Cleared UI panel
        [SerializeField] private GameObject winPanel;
        [Header("Particles")]
        [SerializeField] private ParticleSystem shotPrefab;
        [SerializeField] private ParticleSystem burstPrefab;
        [SerializeField] private SpriteRenderer movePrefab;
        [SerializeField] private float shotDuration;
        [SerializeField] private float moveDuration;
        [SerializeField] private Gradient numberColors;
        [SerializeField] private int maxNumberToColor = 100;

        [SerializeField, HideInInspector] private List<CellVisual> cells = new List<CellVisual>();

        private Grid grid;
        private Vector2Int selectedCell;

        private float Timer;
        [SerializeField] private float TickInterval;
        
        // Flag to prevent further ticks after winning
        private bool isGameWon = false;

        [ContextMenu("Generate")]
        private void GenerateCells()
        {
            if (cells == null)
                cells = new List<CellVisual>();
            foreach (var cell in cells)
                if (cell != null)
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
            if (cells == null)
                cells = new List<CellVisual>();
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
                if (cell != null)
                    DestroyImmediate(cell.gameObject);
        }

        private void Awake()
        {
            RegenerateCells();
            selectedCell = gridSize / 2;
            grid = new Grid(gridSize);
            grid.OnShot += GridOnShot;
            grid.OnMove += GridOnMove;
            foreach (var cell in cells)
                cell.Setup(grid);
            GetCell(selectedCell).HighliteCell();
            foreach (var b in buildCommands)
            {
                b.OnBuildCommand += BuildTower;
                b.Validate += ValidateBuild;
            }
            foreach (var b in buildModifierCommands)
            {
                b.OnBuildCommand += BuildModifier;
                b.Validate += ValidateBuild;
            }
            shootCommand.OnCommand += ActivateTower;
            shootCommand.Validate += ValidateShoot;
            shootAllCommand.OnCommand += ActivateAll;
            shootAllCommand.Validate += ValidateShootAll;
            upgradeCommand.OnCommand += UpgradeTower;
            upgradeCommand.Validate += ValidateUpgrade;
            
            // Hide the win panel on start
            if (winPanel != null)
            {
                winPanel.SetActive(false);
            }
        }

        private void GridOnMove(Vector2Int from, Vector2Int to, int val)
        {
            if (movePrefab == null)
                return;

            var ps = Instantiate(movePrefab);
            ps.color = GetNumberColor(val);
            ps.transform.position = GetCell(from).transform.position;

            ps.transform.DOMove(GetCell(to).transform.position, moveDuration * Random.Range(0.75f, 1.25f))
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    Destroy(ps);
                });
        }

        private void GridOnShot(Vector2Int from, Vector2Int to, int damage, int aoe)
        {
            if (shotPrefab == null)
                return;

            var ps = Instantiate(shotPrefab);
            ps.transform.position = GetCell(from).transform.position;

            ps.transform.DOMove(GetCell(to).transform.position, shotDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    var ps2 = Instantiate(burstPrefab, GetCell(to).transform.position, Quaternion.identity, null);
                    var main = ps2.main;
                    var limitVelocity = ps2.limitVelocityOverLifetime;
                    var emission = ps2.emission;

                    float fixedLifetime = main.startLifetime.constant;
                    float calculatedSpeed = aoe / fixedLifetime;
                    float dragForce = limitVelocity.dampen;
                    main.startSpeed = calculatedSpeed / (1f - dragForce);

                    ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
                    emission.GetBursts(bursts);
                    bursts[0].minCount = (short)(bursts[0].minCount * damage * aoe);
                    bursts[0].maxCount = (short)(bursts[0].maxCount * damage * aoe);
                    emission.SetBursts(bursts);

                    ps2.Play();
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    Destroy(ps.gameObject, 3.0f);
                });
        }

        private bool ValidateBuild()
        {
            return grid.GetCell(selectedCell).Content == null;
        }

        private bool ValidateShoot()
        {
            return grid.GetCell(selectedCell).Content is TowerEntity;
        }

        private bool ValidateShootAll()
        {
            return true;
        }

        private bool ValidateUpgrade()
        {
            return grid.GetCell(selectedCell).Content is TowerEntity;
        }

        private void Start()
        {
            foreach (var cell in cells)
                cell.PreUpdateViz();
            foreach (var cell in cells)
                cell.UpdateVisual(grid.GetCell(cell.Pos));
        }

        private void OnDestroy()
        {
            foreach (var b in buildCommands)
            {
                b.OnBuildCommand -= BuildTower;
                b.Validate -= ValidateBuild;
            }
            foreach (var b in buildModifierCommands)
            {
                b.OnBuildCommand -= BuildModifier;
                b.Validate -= ValidateBuild;
            }
            shootCommand.OnCommand -= ActivateTower;
            shootCommand.Validate -= ValidateShoot;
            shootAllCommand.OnCommand -= ActivateAll;
            shootAllCommand.Validate -= ValidateShootAll;
            upgradeCommand.OnCommand -= UpgradeTower;
            upgradeCommand.Validate -= ValidateUpgrade;
        }

        private void Update()
        {
            // Stop logic if the level is already cleared
            if (isGameWon) return;

            Timer += Time.deltaTime;
            bool showLHint = selectedCell.x > gridSize.x / 2;
            hintL?.SetActive(showLHint);
            hintR?.SetActive(!showLHint);

            if (Timer >= TickInterval)
            {
                grid.Tick();
                foreach (var cell in cells)
                    cell.PreUpdateViz();
                foreach (var cell in cells)
                    cell.UpdateVisual(grid.GetCell(cell.Pos));
                Timer = 0;
                
                // Check win condition after each tick
                if (grid.IsLevelCleared())
                {
                    HandleWin();
                }
            }
        }

        // Handles the level clear event
        [ContextMenu("Win")]
        public void HandleWin()
        {
            isGameWon = true;
            Debug.Log("LEVEL CLEARED! Showing Win Panel.");
            
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }

        private CellVisual GetCell(Vector2Int pos)
        {
            return cells[pos.y + pos.x * gridSize.y];
        }

        void BuildTower(TowerSO tower)
        {
            Debug.Log("Build tower");

            if (ValidateBuild())
            {
                tower.Build(grid, selectedCell);
                // use resources
                GetCell(selectedCell).UpdateVisual(grid.GetCell(selectedCell));
            }
        }

        void BuildModifier(ModifierCellSO modifier)
        {
            if (grid.GetCell(selectedCell).Content == null)
            {
                modifier.Setup(grid, selectedCell);
                modifier.SetupUI(GetCell(selectedCell));
                // use resources
                GetCell(selectedCell).UpdateVisual(grid.GetCell(selectedCell));
            }
        }

        void ActivateTower()
        {
            if (ValidateShoot())
            {
                grid.Command(shootCommand, selectedCell);
                // use resources
                foreach (var cell in cells)
                    cell.PreUpdateViz();
                foreach (var cell in cells)
                    cell.UpdateVisual(grid.GetCell(cell.Pos));
            }
        }

        void ActivateAll()
        {
            Debug.Log("Activate all");

            grid.Command(shootAllCommand, selectedCell);
            // use resources
            foreach (var cell in cells)
                cell.PreUpdateViz();
            foreach (var cell in cells)
                cell.UpdateVisual(grid.GetCell(cell.Pos));
        }

        void UpgradeTower()
        {
            if (ValidateUpgrade())
            {
                grid.Command(upgradeCommand, selectedCell);
                // use resources
                foreach (var cell in cells)
                    cell.PreUpdateViz();
                foreach (var cell in cells)
                    cell.UpdateVisual(grid.GetCell(cell.Pos));
            }
        }

        public void MoveUp()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.y = (selectedCell.y + 1 + gridSize.y) % gridSize.y;
            Debug.Log(selectedCell);

            // we skip void cell
            if (grid.GetCell(selectedCell).Content is VoidEntity ve)
            {
                MoveUp();
                return;
            }
            foreach (var cell in cells)
                cell.PreUpdateViz();
            foreach (var cell in cells)
                cell.UpdateVisual(grid.GetCell(cell.Pos));
            GetCell(selectedCell).HighliteCell();
        }

        public void MoveDown()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.y = (selectedCell.y - 1 + gridSize.y) % gridSize.y;
            Debug.Log(selectedCell);

            // we skip void cell
            if (grid.GetCell(selectedCell).Content is VoidEntity ve)
            {
                MoveDown();
                return;
            }
            foreach (var cell in cells)
                cell.PreUpdateViz();
            foreach (var cell in cells)
                cell.UpdateVisual(grid.GetCell(cell.Pos));
            GetCell(selectedCell).HighliteCell();
        }

        public void MoveLeft()
        {
            GetCell(selectedCell).DeselectCell();

            selectedCell.x = (selectedCell.x - 1 + gridSize.x) % gridSize.x;
            Debug.Log(selectedCell);

            // we skip void cell
            if (grid.GetCell(selectedCell).Content is VoidEntity ve)
            {
                MoveLeft();
                return;
            }
            foreach (var cell in cells)
                cell.PreUpdateViz();
            foreach (var cell in cells)
                cell.UpdateVisual(grid.GetCell(cell.Pos));
            GetCell(selectedCell).HighliteCell();
        }

        public void MoveRight()
        {
            GetCell(selectedCell).DeselectCell();
            selectedCell.x = (selectedCell.x + 1 + gridSize.x) % gridSize.x;
            Debug.Log(selectedCell);

            // we skip void cell
            if (grid.GetCell(selectedCell).Content is VoidEntity ve)
            {
                MoveRight();
                return;
            }
            foreach (var cell in cells)
                cell.PreUpdateViz();
            foreach (var cell in cells)
                cell.UpdateVisual(grid.GetCell(cell.Pos));
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

        public void InTowerRange(Vector2Int pos, int range)
        {
            if (pos != selectedCell)
                return;
            for (int y = pos.y - range; y <= pos.y + range; y++)
            {
                for (int x = pos.x + range; x >= pos.x - range; x--)
                {
                    Vector2Int checkPos = new Vector2Int(x, y);
                    if (!grid.IsValidPos(checkPos)) continue;
                    int distance = Mathf.Abs(x - pos.x) + Mathf.Abs(y - pos.y);
                    if (distance > range) continue;
                    GetCell(new Vector2Int(x, y)).InTowerRange();
                }
            }
        }

        public Color GetNumberColor(int val)
        {
            if (val > maxNumberToColor)
                val = maxNumberToColor;
            float t = val / (float)maxNumberToColor;
            return numberColors.Evaluate(t);
        }
    }
}