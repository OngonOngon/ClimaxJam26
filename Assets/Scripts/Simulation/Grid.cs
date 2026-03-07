using UnityEngine;

namespace Dubinci
{
    public class Grid
    {
        private Vector2Int dim;
        private Cell[,] cells; // back buffer
        private Cell[,] nextCells; // front buffer

        public Grid(Vector2Int dim)
        {
            this.dim = dim;

            cells = new Cell[dim.x, dim.y];
            nextCells = new Cell[dim.x, dim.y];
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    cells[x, y] = new Cell();
                    nextCells[x, y] = new Cell();
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

        public void AddModifierAt(ModifierType type, int value, Vector2Int pos)
        {
            if (!IsValidPos(pos))
            {
                return;
            }
            // if already has modifier, return
            if (this.cells[pos.x, pos.y].modifier.type != ModifierType.None)
            {
                return;
            }


            this.cells[pos.x, pos.y].CreateModifier(type, value);
            this.nextCells[pos.x, pos.y].CreateModifier(type, value);
        }

        public void AddTowerAt(char letter, int damage, int range, int hp, Vector2Int pos)
        {
            if (!IsValidPos(pos))
            {
                return;
            }

            Cell targetCell = this.cells[pos.x, pos.y];

            targetCell.Content = new TowerEntity(letter, damage, range, hp);
        }

        public bool IsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < dim.x && pos.y < dim.y;
        }
        public void Tick()
        {
            Debug.Log("Tickiiiing!!!!!!!");
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    if (cells[x, y].Content is TowerEntity)
                    {
                        nextCells[x, y].Content = cells[x, y].Content;
                    }
                    else
                    {
                        nextCells[x, y].Content = null;
                    }
                }
            }

            // for each cell in current grid, spread its contents into the surrounding in nextCells
            // order of spreading: 1) top, 2) top right, 3) top left, 4) left, 5) right, 6) bottom left, 7) bottom right, 8) bottom
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    Cell currentCell = this.cells[x, y];

                    // check what type the content of a cell is
                    if (currentCell.Content is NumberEntity numberEntity)
                    {
                        // spread the number to the surrounding cells in nextCells
                        Vector2Int[] directions = new Vector2Int[]
                        {
                            new Vector2Int(0, 1), // top
                            new Vector2Int(1, 1), // top right
                            new Vector2Int(-1, 1), // top left
                            new Vector2Int(-1, 0), // left
                            new Vector2Int(1, 0), // right
                            new Vector2Int(-1, -1), // bottom left
                            new Vector2Int(1, -1), // bottom right
                            new Vector2Int(0, -1), // bottom
                            new Vector2Int(0, 0) // stay
                        };
                        int currDirectionIndex = 0;

                        // print x y
                        while (numberEntity.Value > 0)
                        {
                            // print trying direction

                            Vector2Int targetPos = new Vector2Int(x, y) + directions[currDirectionIndex];
                            if (IsValidPos(targetPos))
                            {
                                Cell targetCell = nextCells[targetPos.x, targetPos.y];

                                switch (targetCell.Content)
                                {
                                    case null: // empty cell
                                        NumberEntity newNumberEntity = new NumberEntity(0);
                                        switch (targetCell.GetModifier())
                                        {
                                            case Modifier { type: ModifierType.Add, value: var addValue }:
                                                newNumberEntity.Value = 1;
                                                newNumberEntity.addThisTick = true;
                                                break;
                                            case Modifier { type: ModifierType.Multiply, value: var mulValue }:
                                                newNumberEntity.Value += 1 * mulValue;
                                                break;
                                            case Modifier { type: ModifierType.None, value: _ }:
                                                newNumberEntity.Value = 1;
                                                break;
                                        }
                                        targetCell.Content = newNumberEntity;
                                        break;
                                    case NumberEntity targetNumberEntity: // already a number cell
                                        switch (targetCell.GetModifier())
                                        {
                                            case Modifier { type: ModifierType.Add, value: _ }:
                                                targetNumberEntity.Value += 1;
                                                targetNumberEntity.addThisTick = true;
                                                break;
                                            case Modifier { type: ModifierType.Multiply, value: var mulValue }:
                                                targetNumberEntity.Value += 1 * mulValue;
                                                break;
                                            case Modifier { type: ModifierType.None, value: _ }:
                                                targetNumberEntity.Value++;
                                                break;
                                        }
                                        break;
                                    case TowerEntity towerEntity: // tower cell
                                        towerEntity.HP -= 1;
                                        if (towerEntity.HP <= 0)
                                        {
                                            targetCell.Content = null;
                                        }
                                        break;
                                }
                                numberEntity.Value--;
                            }

                            currDirectionIndex += 1;
                            currDirectionIndex %= directions.Length;
                        }
                    }
                }
            }

            // add modifier value to all numbers that got added this tick
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    if (nextCells[x, y].Content is NumberEntity num && num.addThisTick)
                    {
                        switch (nextCells[x, y].GetModifier())
                        {
                            case Modifier { type: ModifierType.Add, value: var addValue }:
                                num.Value += addValue;
                                break;
                        }
                        num.addThisTick = false;
                    }
                }
            }

            // clearing zeros
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    if (nextCells[x, y].Content is NumberEntity num && num.Value <= 0)
                    {
                        nextCells[x, y].Content = null;
                    }
                }
            }

            // update cells and reset nextCells
            Cell[,] temp = cells;
            cells = nextCells;
            nextCells = temp;
        }

        private Cell GetTarget(int range, Vector2Int pos)
        {
            Cell bestTarget = null;
            int maxNumber = 0;

            for (int y = pos.y - range; y <= pos.y + range; y++)
            {
                for (int x = pos.x + range; x >= pos.x - range; x--)
                {
                    Vector2Int checkPos = new Vector2Int(x, y);

                    if (!IsValidPos(checkPos)) continue;

                    if (Mathf.Abs(x - pos.x) + Mathf.Abs(y - pos.y) > range) continue;

                    Cell currentCell = cells[checkPos.x, checkPos.y];
                    if (currentCell.Content is NumberEntity numberEntity && numberEntity.Value > maxNumber)
                    {
                        maxNumber = numberEntity.Value;
                        bestTarget = currentCell;
                    }
                }
            }

            return bestTarget;
        }
        public void Command(CommandSO command, Vector2Int pos)
        {
            if (!IsValidPos(pos))
            {
                return;
            }

            Debug.Log($"Executing command {command.type} at position {pos}");

            switch (command.type)
            {
                case CommandType.Shoot:
                    Cell currentCell = cells[pos.x, pos.y];
                    if (currentCell.Content is TowerEntity tower)
                    {
                        Cell hitCell = GetTarget(tower.Range, pos);

                        if (hitCell.Content is NumberEntity number)
                        {
                            number.Value -= tower.Damage;
                            if (number.Value <= 0)
                            {
                                // make the cell be null
                                hitCell.Content = null;
                            }
                        }
                    }
                    break;
            }
        }
        public Cell GetCell(Vector2Int pos)
        {
            return cells[pos.x, pos.y];
        }
    }
}
