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
    }

    public class LetterEntity : IGridEntity
    {
        public char Letter;
        public LetterEntity(char letter) { Letter = letter; }
        public string GetContentString() => Letter.ToString();
    }

    public class Cell
    {
        public IGridEntity Content = null;

        public bool IsEmpty()
        {
            return Content == null;
        }

        public string GetContentString()
        {
            return IsEmpty() ? "." : Content.GetContentString();
        }
    }
}
