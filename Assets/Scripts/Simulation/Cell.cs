using UnityEngine;

namespace Dubinci
{
    public interface IGridEntity
    {
        string GetContentString();
    }

    public class VoidEntity : IGridEntity
    {
        public string GetContentString() => "~";
    }

    public class NumberEntity : IGridEntity
    {
        public int Value;
        public NumberEntity(int val) { Value = val; }
        public string GetContentString() => Value.ToString();

        public bool addThisTick = false;
    }

    public enum TowerType
    {
        Basic,
        Auto,
    };

    public class TowerEntity : IGridEntity
    {
        public char Letter;
        public int Damage;
        public int Range;
        public int HP;
        public int AOE;

        // example of usage:
        // Initial Tower Stats:
        //          Damage: 3
        //          DmgUpgrade: 2
        //          DmgUpgradeThreshold: 3
        //          Threshold counter: 0
        // Upgrade 1:
        //          Damage: 5
        //          DmgUpgrade: 2
        //          DmgUpgradeThreshold: 3
        //          Threshold counter: 1
        // Upgrade 2:
        //          Damage: 7
        //          DmgUpgrade: 2
        //          DmgUpgradeThreshold: 3
        //          Threshold counter: 2
        // Upgrade 3:
        //          Damage: 10
        //          DmgUpgrade: 3
        //          DmgUpgradeThreshold: 3
        //          Threshold counter: 0
        public int UpgradeAmountDamage;
        public int UpgradeAmountDamageThreshold;
        private int damageCounter = 0;

        public int UpgradeAmountRange;
        public int UpgradeAmountRangeThreshold;
        private int rangeCounter = 0;

        public int UpgradeAmountHP;
        public int UpgradeAmountHPThreshold;
        private int hpCounter = 0;

        public int UpgradeAmountAOE;
        public int UpgradeAmountAOEThreshold;
        private int aoeCounter = 0;

        public int ThresholdLevel = 0;

        public TowerType Type;
        public TowerEntity(TowerType type, char letter, int damage, int range, int hp, int aoe, TowerUpgradeConfig upgrades)
        {
            Type = type;
            Letter = letter;
            Damage = damage;
            Range = range;
            HP = hp;
            AOE = aoe;

            UpgradeAmountDamage = upgrades.DamageAmount;
            UpgradeAmountDamageThreshold = upgrades.DamageThreshold;

            UpgradeAmountRange = upgrades.RangeAmount;
            UpgradeAmountRangeThreshold = upgrades.RangeThreshold;

            UpgradeAmountHP = upgrades.HPAmount;
            UpgradeAmountHPThreshold = upgrades.HPThreshold;

            UpgradeAmountAOE = upgrades.AOEAmount;
            UpgradeAmountAOEThreshold = upgrades.AOEThreshold;
        }

        // letter + HP
        public string GetContentString() => $"{Letter}";

        public void Upgrade()
        {
            ApplyStatUpgrade(ref Damage, ref UpgradeAmountDamage, UpgradeAmountDamageThreshold, ref damageCounter);
            ApplyStatUpgrade(ref Range, ref UpgradeAmountRange, UpgradeAmountRangeThreshold, ref rangeCounter);
            ApplyStatUpgrade(ref HP, ref UpgradeAmountHP, UpgradeAmountHPThreshold, ref hpCounter);
            ApplyStatUpgrade(ref AOE, ref UpgradeAmountAOE, UpgradeAmountAOEThreshold, ref aoeCounter);
        }

        private void ApplyStatUpgrade(ref int stat, ref int upgradeAmount, int threshold, ref int counter)
        {
            if (threshold <= 0) return;

            counter++;

            if (counter >= threshold)
            {
                upgradeAmount++;
                counter = 0;
            }

            stat += upgradeAmount;
        }
    }

    public class PlayerBase : IGridEntity
    {
        public string GetContentString() => "$";

        public PlayerBase() { }

        public void DamageBase()
        {
            playerResources resources = Object.FindAnyObjectByType<playerResources>();

            resources.TakeDamage(1);
        }

    }

    public class Cell
    {
        public Vector2Int Position;
        public IGridEntity Content = null;

        public Modifier modifier;

        public Cell()
        {
            modifier = new Modifier { type = ModifierType.None, value = 0 };
        }

        public bool IsEmpty()
        {
            return Content == null;
        }

        public Modifier GetModifier()
        {
            return modifier;
        }

        public string GetContentString()
        {
            if (IsEmpty())
            {
                return "";
            }

            return Content.GetContentString();
        }

        public void CreateModifier(ModifierType type, int value)
        {
            modifier.type = type;
            modifier.value = value;
        }
    }

    public enum ModifierType
    {
        None = -1,
        Add = 0,
        Multiply = 1,
        // Player-built
        Subtract = 2,
        Divide = 3
    }

    public class Modifier
    {
        public ModifierType type;
        public int value;
    }
}
