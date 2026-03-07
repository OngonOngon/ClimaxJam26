using UnityEngine;
using TMPro;

namespace Dubinci
{
    public class CellVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueTXT;
        [SerializeField] private GameObject modifPanel;
        [SerializeField] private TextMeshProUGUI modifTXT;
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

            name = $"[{pos.x},{pos.y}] {value.name}";
            value.SetupUI(this);
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
            value.SetupUI(this);
        }

        public void SetMainText(string text)
        {
            if (valueTXT)
                valueTXT.text = text;
        }

        public void ShowModifier(string text)
        {
            modifPanel.SetActive(true);
            modifTXT.text = text;
        }

        public void HideModifier()
        {
            modifPanel.SetActive(false);
        }

        public void ClearViz()
        {
            valueTXT.text = "";
            HideModifier();
        }
    }
}
