using Cysharp.Threading.Tasks;
using Foundation.GUI.Views;
using Foundation.Inventory.Components;
using Foundation.Inventory.Configs;
using Foundation.Items.Components;
using Foundation.Items.Tags;
using Foundation.Shared;
using Foundation.SpawnSystem.Components;
using Leopotam.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Foundation.Inventory.Systems
{
    sealed class InventoryControlSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<ItemTag, SpawnableComponent,
            ObtainableComponent> _obtainableFilter = null;
        private readonly EcsFilter<ItemTag, SpawnableComponent,
            RemovableComponent> _removableFilter = null;

        private ItemInventoryConfig _itemInventoryConfig;
        private InventoryPanelView _inventoryPanelView;

        private GameObject _itemInventoryViewPrefab;
        private CancellationTokenSource _cancellationTokenSource;

        private Stack<ItemInventoryView> _itemInventoryViews;
        private List<ItemInventoryView> _disabledInventoryViews;

        public void Init()
        {
            _itemInventoryViews = new Stack<ItemInventoryView>();
            _disabledInventoryViews = new List<ItemInventoryView>();
            _cancellationTokenSource = new CancellationTokenSource();

            _inventoryPanelView.InitializeButton(OnDropButtonClicked);

            LoadPrefab(_itemInventoryConfig.ItemViewPrefab, 
                _cancellationTokenSource.Token).Forget();
        }

        public void Run()
        {
            foreach (var entity in _obtainableFilter)
            {
                ref var obtainable = ref _obtainableFilter.Get3(entity);

                if (obtainable.IsObtained)
                {
                    obtainable.IsObtained = false;

                    ref var spawnable = ref _obtainableFilter.Get2(entity);

                    AddItem(spawnable.Guid, obtainable.InventoryIcon);
                }
            }
        }

        public void Destroy()
        {
            if (_cancellationTokenSource != null && 
                _cancellationTokenSource.IsCancellationRequested == false)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }

            _inventoryPanelView.UnsubscribeButton(OnDropButtonClicked);
        }

        public void OnDropButtonClicked()
        {
            var itemInventoryView = _itemInventoryViews.Pop();

            itemInventoryView.gameObject.SetActive(false);
            _disabledInventoryViews.Add(itemInventoryView);
            
            foreach (var entity in _removableFilter)
            {
                ref var spawnable = ref _removableFilter.Get2(entity);

                if (spawnable.Guid == itemInventoryView.Guid)
                {
                    ref var removable = ref _removableFilter.Get3(entity);

                    removable.IsRemoved = true;
                }
            }

            _inventoryPanelView.TryDisableDropButton(_itemInventoryViews.Count);
        }

        private void AddItem(Guid guid, Sprite sprite)
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
            _inventoryPanelView.TryEnableDropButton();
        }

        private ItemInventoryView CreateItemInventoryView()
        {
            return UnityEngine.Object.Instantiate(_itemInventoryViewPrefab, _inventoryPanelView.ItemViewsContainer).
                GetComponent<ItemInventoryView>();
        }

        private async UniTask LoadPrefab(CustomAssetReferenceTo<GameObject> prefab, 
            CancellationToken cancellationToken)
        {
            _itemInventoryViewPrefab = await Addressables.LoadAssetAsync<GameObject>(prefab)
                .ToUniTask(cancellationToken: cancellationToken);

            _cancellationTokenSource = null;
        }
    }
}