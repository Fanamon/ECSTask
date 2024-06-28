using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Foundation.GUI.Views
{
    public class InventoryPanelView : MonoBehaviour
    {
        [SerializeField] private Button _dropItemButton;
        [SerializeField] private GameObject _itemsViewsContainer;
        [SerializeField] private ItemInventoryView _itemInventoryViewPrefab;

        private Stack<ItemInventoryView> _itemInventoryViews;
        private List<ItemInventoryView> _disabledInventoryViews;

        public event UnityAction<Guid> ItemRemoved;

        private void Awake()
        {
            _itemInventoryViews = new Stack<ItemInventoryView>();
            _disabledInventoryViews = new List<ItemInventoryView>();
            _dropItemButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _dropItemButton.onClick.AddListener(OnDropButtonClicked);
        }

        private void OnDisable()
        {
            _dropItemButton.onClick.RemoveListener(OnDropButtonClicked);
        }

        public void AddItem(Guid guid, Sprite sprite)
        {
            ItemInventoryView itemInventoryView = _disabledInventoryViews.FirstOrDefault();

            if (itemInventoryView == null)
            {
                itemInventoryView = CreateItemInventoryView();
            }
            else
            {
                _disabledInventoryViews.Remove(itemInventoryView);
                itemInventoryView.gameObject.SetActive(true);
            }

            itemInventoryView.Initialize(guid, sprite);
            _itemInventoryViews.Push(itemInventoryView);

            if (_dropItemButton.gameObject.activeSelf == false)
            {
                _dropItemButton.gameObject.SetActive(true);
            }
        }

        public void OnDropButtonClicked()
        {
            var itemInventoryView = _itemInventoryViews.Pop();

            itemInventoryView.gameObject.SetActive(false);
            _disabledInventoryViews.Add(itemInventoryView);
            ItemRemoved?.Invoke(itemInventoryView.Guid);

            if (_itemInventoryViews.Count == 0)
            {
                _dropItemButton.gameObject.SetActive(false);
            }
        }

        public ItemInventoryView CreateItemInventoryView()
        {
            return Instantiate(_itemInventoryViewPrefab, _itemsViewsContainer.transform);
        }
    }
}