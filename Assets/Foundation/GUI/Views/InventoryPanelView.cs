using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Foundation.GUI.Views
{
    public class InventoryPanelView : MonoBehaviour
    {
        [SerializeField] private Button _dropItemButton;
        [SerializeField] private GameObject _itemsViewsContainer;

        public Transform ItemViewsContainer => _itemsViewsContainer.transform;

        public void InitializeButton(UnityAction onButtonClicked)
        {
            _dropItemButton.gameObject.SetActive(false);
            _dropItemButton.onClick.AddListener(onButtonClicked);
        }

        public void UnsubscribeButton(UnityAction onButtonClicked)
        {
            _dropItemButton.onClick.RemoveListener(onButtonClicked);
        }

        public bool TryEnableDropButton()
        {
            bool isButtonEnabled = false;

            if (_dropItemButton.gameObject.activeSelf == false)
            {
                _dropItemButton.gameObject.SetActive(true);

                isButtonEnabled = true;
            }

            return isButtonEnabled;
        }

        public bool TryDisableDropButton(int itemViewsCount)
        {
            bool isButtonDisabled = false;

            if (itemViewsCount == 0)
            {
                _dropItemButton.gameObject.SetActive(false);

                isButtonDisabled = true;
            }

            return isButtonDisabled;
        }
    }
}