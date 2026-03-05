using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Pospec.Popups
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textUI;

        public void ShowPopup(PopupSetter setter)
        {
            gameObject.SetActive(true);
        }

        public class PopupSetter
        {
            private Popup popup;
            private string popupTitle;
            private List<PopupButtonBuilder> buttons;

            public PopupSetter(Popup popup, string popupTitle)
            {
                this.popupTitle = popupTitle;
            }

            public void AddButton(PopupButtonBuilder builder)
            {
                buttons.Add(builder);
            }
            
            public void ShowPopup()
            {
                popup.textUI.text = popupTitle;
            }
        }
    }
}
