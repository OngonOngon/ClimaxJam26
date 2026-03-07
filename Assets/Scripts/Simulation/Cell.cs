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
        public TowerType Type;
        public TowerEntity(TowerType type, char letter, int damage, int range, int hp, int aoe)
        {
            Letter = letter;
            Damage = damage;
            Range = range;
            HP = hp;
            AOE = aoe;
            Type = type;
        }

        // letter + HP
        public string GetContentString() => $"{Letter}";
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
