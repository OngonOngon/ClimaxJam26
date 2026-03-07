using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Dubinci
{
    public class CellVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueTXT;
        [SerializeField] private GameObject modifPanel;
        [SerializeField] private TextMeshProUGUI modifTXT;
        [SerializeField] private CellValueSO value;
        [SerializeField] private Image blackOverlay;
        [SerializeField] private Image selectIMG;
        [SerializeField] private Image mainImage;
        [SerializeField] private Color selectedCol = Color.white;
        [SerializeField] private Color highlitedCol = Color.white;
        [SerializeField] private TMP_FontAsset numberSDF;
        [SerializeField] private TMP_FontAsset letterSDF;
        [SerializeField] private Color numberColor;
        [SerializeField] private Color letterColor;

        [SerializeField, HideInInspector] private Vector2Int pos;
        [SerializeField, HideInInspector] private GridVisual grid;

        public Vector2Int Pos => pos;

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

        public void UpdateVisual(Cell cell)
        {
            if (cell.Content is Dubinci.VoidEntity)
            {
                SetMainText("");
                if (mainImage)
                {
                    mainImage.enabled = false;
                }
                return;
            }

            SetMainText(cell.GetContentString());
            if (cell.Content != null && cell.Content is Dubinci.NumberEntity)
            {
                valueTXT.font = numberSDF;
                valueTXT.color = numberColor;
                blackOverlay.gameObject.SetActive(true);
            }
            else
            {
                valueTXT.font = letterSDF;
                valueTXT.color = letterColor;
                blackOverlay.gameObject.SetActive(false);
            }
        }

        public void HighliteCell()
        {
            selectIMG.gameObject.SetActive(true);
            selectIMG.color = highlitedCol;
        }

        public void SelectCell()
        {
            selectIMG.gameObject.SetActive(true);
            selectIMG.color = selectedCol;
        }

        public void DeselectCell()
        {
            selectIMG.gameObject.SetActive(false);
        }
    }
}
