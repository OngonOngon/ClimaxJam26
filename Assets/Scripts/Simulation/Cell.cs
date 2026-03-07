using UnityEngine;

namespace Dubinci
{
    public interface IGridEntity
    {
        string GetContentString();
    }

    public class NumberEntity : IGridEntity
    {
        public int Value;
        public NumberEntity(int val) { Value = val; }
        public string GetContentString() => Value.ToString();

        public bool addThisTick = false;
    }

    public class TowerEntity : IGridEntity
    {
        public char Letter;
        public TowerEntity(char letter) { Letter = letter; }
        public string GetContentString() => Letter.ToString();
    }

    public class Cell
    {
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
            // if empty and has modifier, show modifier
            if (IsEmpty() && modifier.type != ModifierType.None)
            {
                return modifier.type == ModifierType.Add ? $"A{modifier.value}" : $"M{modifier.value}";
            }
            return IsEmpty() ? "." : Content.GetContentString();
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
        Multiply = 1
    }

    public class Modifier
    {
        public ModifierType type;
        public int value;
    }
}
