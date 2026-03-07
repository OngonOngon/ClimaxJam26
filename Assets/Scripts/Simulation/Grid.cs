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
                    cells[x, y] = new Cell
                    {
                        Position = new Vector2Int(x, y)
                    };
                    nextCells[x, y] = new Cell
                    {
                        Position = new Vector2Int(x, y)
                    };
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

            if (number == -1)
            {
                targetCell.Content = new VoidEntity();
                return;
            }

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

        public void AddTowerAt(TowerType type, char letter, int damage, int range, int hp, int aoe, Vector2Int pos)
        {
            if (!IsValidPos(pos))
            {
                return;
            }

            Cell targetCell = this.cells[pos.x, pos.y];

            targetCell.Content = new TowerEntity(type, letter, damage, range, hp, aoe);
        }

        public bool IsValidPos(Vector2Int pos)
        {
            if (!(pos.x >= 0 && pos.y >= 0 && pos.x < dim.x && pos.y < dim.y))
            {
                return false;
            }
            if (cells[pos.x, pos.y].Content is VoidEntity)
            {
                return false;
            }

            return true;
        }
        public void Tick()
        {
            Debug.Log("Tickiiiing!!!!!!!");
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    IGridEntity content = cells[x, y].Content;
                    if (content is TowerEntity || content is VoidEntity)
                    {
                        nextCells[x, y].Content = content;
                    }
                    else
                    {
                        nextCells[x, y].Content = null;
                    }
                }
            }

            // first shoot, only then move
            for (int x = 0; x < dim.x; x++)
            {
                for (int y = 0; y < dim.y; y++)
                {
                    Cell currentCell = this.cells[x, y];

                    if (currentCell.Content is TowerEntity towerEntity && towerEntity.Type == TowerType.Auto)
                    {
                        // create shoot command and execute it
                        CommandSO cmd = ScriptableObject.CreateInstance<CommandSO>();
                        cmd.changeType(CommandType.Shoot);
                        Command(cmd, currentCell.Position);
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
                            case Modifier { type: ModifierType.Subtract, value: var subValue }:
                                num.Value -= subValue;
                                break;
                            case Modifier { type: ModifierType.Divide, value: var divValue }:
                                num.Value = Mathf.FloorToInt((float)num.Value / divValue);
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
            int maxNumber = -1;
            int minDistance = int.MaxValue;

            for (int y = pos.y - range; y <= pos.y + range; y++)
            {
                for (int x = pos.x + range; x >= pos.x - range; x--)
                {
                    Vector2Int checkPos = new Vector2Int(x, y);

                    if (!IsValidPos(checkPos)) continue;

                    int distance = Mathf.Abs(x - pos.x) + Mathf.Abs(y - pos.y);

                    if (distance > range) continue;

                    Cell currentCell = cells[checkPos.x, checkPos.y];
                    if (currentCell.Content is NumberEntity numberEntity)
                    {
                        if (numberEntity.Value > maxNumber)
                        {
                            maxNumber = numberEntity.Value;
                            minDistance = distance;
                            bestTarget = currentCell;
                        }
                        else if (numberEntity.Value == maxNumber && distance < minDistance)
                        {
                            minDistance = distance;
                            bestTarget = currentCell;
                        }
                    }
                }
            }

            return bestTarget;
        }
        private void Shoot(Vector2Int pos)
        {
            Cell currentCell = cells[pos.x, pos.y];
            if (currentCell.Content is TowerEntity tower)
            {
                Cell hitCell = GetTarget(tower.Range, pos);

                if (hitCell == null)
                {
                    Debug.Log("No target in range");
                    return;
                }

                Vector2Int targetPos = hitCell.Position;

                // apply aoe
                for (int y = targetPos.y - tower.AOE; y <= targetPos.y + tower.AOE; y++)
                {
                    for (int x = targetPos.x + tower.AOE; x >= targetPos.x - tower.AOE; x--)
                    {
                        Vector2Int checkPos = new Vector2Int(x, y);

                        if (!IsValidPos(checkPos)) continue;


                        Cell aoeCell = cells[checkPos.x, checkPos.y];
                        if (aoeCell.Content is NumberEntity numberEntity)
                        {
                            numberEntity.Value -= tower.Damage;

                            if (numberEntity.Value <= 0)
                            {
                                // make the cell be null
                                aoeCell.Content = null;
                            }
                        }
                    }
                }


            }
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
                    Shoot(pos);
                    break;
                case CommandType.ShootAll:
                    for (int x = pos.x; x >= pos.x; x--)
                    {
                        for (int y = pos.y; y <= pos.y; y++)
                        {
                            Vector2Int checkPos = new Vector2Int(x, y);

                            if (!IsValidPos(checkPos)) continue;

                            Shoot(checkPos);
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
