using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Dubinci
{
    public class textHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Color hoverColor = Color.gray;
        
        private TextMeshProUGUI _text;
        private Color _originalColor;

        void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            
            if (_text != null)
            {
                _originalColor = _text.color;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_text != null)
            {
                _text.color = hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_text != null)
            {
                _text.color = _originalColor;
            }
        }
    }
}