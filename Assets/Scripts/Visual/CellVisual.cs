using UnityEngine;
using TMPro;

namespace Dubinci
{
    public class CellVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueTMP;
        [SerializeField] private CellValueSO value;
        [SerializeField, HideInInspector] private Vector2Int pos;
        [SerializeField, HideInInspector] private GridVisual grid;

        private void OnValidate()
        {
            if (value == null)
            {
                name = $"C[{pos.x},{pos.y}]";
                return;
            }

            name = $"C[{pos.x},{pos.y}] {value.text}";
            if (valueTMP)
                valueTMP.text = value.text;
        }

        public void Create(GridVisual grid, Vector2Int pos, CellValueSO defaultVal)
        {
            this.grid = grid;
            this.pos = pos;
            value = defaultVal;
            OnValidate();
        }

        public void Create(CellVisual prev)
        {
            Create(prev.grid, prev.pos, prev.value);
        }

        public void Setup(Grid g)
        {
            if (value == null)
                return;
            value.Setup(g, pos);
            valueTMP.text = value.text;
        }
    }
}
