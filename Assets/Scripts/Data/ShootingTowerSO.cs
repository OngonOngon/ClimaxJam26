using UnityEngine;

namespace Dubinci
{
    // Struktura pro přehlednost v Inspectoru
    [System.Serializable]
    public struct TowerUpgradeConfig
    {
        public int DamageAmount;
        public int DamageThreshold;

        public int RangeAmount;
        public int RangeThreshold;

        public int HPAmount;
        public int HPThreshold;

        public int AOEAmount;
        public int AOEThreshold;
    }

    [CreateAssetMenu(fileName = "Shooting Tower", menuName = "Scriptable Objects/Tower/Shooting")]
    public class ShootingTowerSO : TowerSO
    {
        [Header("Base Stats")]
        public TowerType type;
        public int Damage;
        public int Range;
        public int AOE;

        [Header("Upgrade Config")]
        public TowerUpgradeConfig Upgrades;

        public override void Build(Grid grid, Vector2Int pos)
        {
            grid.AddTowerAt(type, Letter, Damage, Range, HP, AOE, Upgrades, pos);
        }

        public override void Activate(Grid grid, Vector2Int pos)
        {
            CommandSO cmd = CreateInstance<CommandSO>();
            cmd.changeType(CommandType.Shoot);
            grid.Command(cmd, pos);
        }
    }
}
