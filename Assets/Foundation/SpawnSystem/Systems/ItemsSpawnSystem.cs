using Cysharp.Threading.Tasks;
using Foundation.GUI.Views;
using Foundation.Inventory.Components;
using Foundation.Items.Configs;
using Foundation.Items.Tags;
using Foundation.Items.Views;
using Foundation.Movement.Components;
using Foundation.Player.Tags;
using Foundation.Shared;
using Foundation.SpawnSystem.Components;
using Foundation.SpawnSystem.Configs;
using Foundation.SpawnSystem.Views;
using Leopotam.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Foundation.SpawnSystem.Systems
{
    sealed class ItemsSpawnSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private EcsWorld _ecsWorld = null;
        private EcsFilter<PlayerTag, StackKeepComponent> _dropPointFilter = null;

        private ItemConfig _itemConfig;
        private SpawnerConfig _spawnerConfig;

        private GameObject _itemPrefab;
        private SpawnContainerView _spawnContainerView;
        private InventoryPanelView _inventoryPanelView;

        private CancellationTokenSource _cancellationTokenSource;

        private float _currentCount;

        private Dictionary<GameObject, EcsEntity> _items;

        public void Init()
        {
            _currentCount = 0;
            _items = new Dictionary<GameObject, EcsEntity>();
            _inventoryPanelView.ItemRemoved += OnItemRemoved;

            if (_cancellationTokenSource != null &&
                _cancellationTokenSource.IsCancellationRequested == false)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            LoadPrefab(_itemConfig.Prefab, _cancellationTokenSource.Token).Forget();
        }

        public void Run()
        {
            if (_currentCount >= _spawnerConfig.Delay)
            {
                _currentCount = 0;

                SpawnItem();
            }
            else
            {
                _currentCount += Time.deltaTime;
            }
        }

        public void Destroy()
        {
            _inventoryPanelView.ItemRemoved -= OnItemRemoved;

            if (_cancellationTokenSource != null &&
                _cancellationTokenSource.IsCancellationRequested == false)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private GameObject SpawnItem(bool isPositionInitialized = false)
        {
            GameObject spawnedItem = _items.Keys.FirstOrDefault(
                item => item.activeSelf == false);

            if (spawnedItem == null)
            {
                spawnedItem = CreateItem(isPositionInitialized);
            }
            else
            {
                ref var spawnable = ref _items[spawnedItem].Get<SpawnableComponent>();
                spawnable.IsPositionRandomized = isPositionInitialized;

                spawnedItem.SetActive(true);
            }

            return spawnedItem;
        }

        private void OnItemRemoved(Guid guid)
        {
            Vector3 dropPoint = Vector3.zero;
            GameObject spawnedItem;

            foreach (var entity in _dropPointFilter)
            {
                ref var stackKeep = ref _dropPointFilter.Get2(entity);

                dropPoint = stackKeep.ItemObtainerView.DropPoint;
            }

            spawnedItem = SpawnItem(true);

            ref var spawnable = ref _items[spawnedItem].Get<SpawnableComponent>();

            spawnable.Guid = guid;
            spawnedItem.GetComponent<ItemView>().Initialize(guid);
            spawnedItem.transform.position = dropPoint;
        }

        private GameObject CreateItem(bool isPositionInitialized = false)
        {
            GameObject item = UnityEngine.Object.Instantiate(_itemPrefab);
            item.transform.parent = _spawnContainerView.transform;
            var itemEntity = CreateItemEntity(item, isPositionInitialized);

            _items.Add(item, itemEntity);

            return item;
        }

        private EcsEntity CreateItemEntity(GameObject item, bool isPositionInitialized = false)
        {
            EcsEntity itemEntity = _ecsWorld.NewEntity();

            ref var itemTag = ref itemEntity.Get<ItemTag>();
            ref var model = ref itemEntity.Get<ModelComponent>();
            ref var droppable = ref itemEntity.Get<DroppableComponent>();
            ref var spawnable = ref itemEntity.Get<SpawnableComponent>();

            model.Transform = item.transform;
            droppable.DropPower = _itemConfig.DropPower;
            droppable.Rigidbody = item.GetComponent<Rigidbody>();
            spawnable.Guid = item.GetComponent<ItemView>().Guid;
            spawnable.IsPositionRandomized = isPositionInitialized;

            return itemEntity;
        }

        private async UniTask LoadPrefab(CustomAssetReferenceTo<GameObject> prefab, CancellationToken cancellationToken)
        {
            _itemPrefab = await Addressables.LoadAssetAsync<GameObject>(prefab)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}