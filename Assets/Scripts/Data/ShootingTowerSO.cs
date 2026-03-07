using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Shooting Tower", menuName = "Scriptable Objects/Tower/Shooting")]
    public class ShootingTowerSO : TowerSO
    {
        public int Damage;
        public int Range;
        public int AOE;
        public TowerType type;

        public override void Build(Grid grid, Vector2Int pos)
        {
            grid.AddTowerAt(type, Letter, Damage, Range, HP, AOE, pos);
        }

        public override void Activate(Grid grid, Vector2Int pos)
        {
            CommandSO cmd = CreateInstance<CommandSO>();
            cmd.changeType(CommandType.Shoot);
            grid.Command(cmd, pos);
        }
    }
}
