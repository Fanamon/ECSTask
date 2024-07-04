using Cysharp.Threading.Tasks;
using Foundation.Inventory.Components;
using Foundation.Items.Components;
using Foundation.Items.Configs;
using Foundation.Items.Tags;
using Foundation.Items.Views;
using Foundation.Movement.Components;
using Foundation.Shared;
using Foundation.SpawnSystem.Components;
using Foundation.SpawnSystem.Configs;
using Foundation.SpawnSystem.Views;
using Leopotam.Ecs;
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
        private readonly EcsFilter<ItemTag, DroppableComponent> _droppableFilter = null;

        private ItemConfig _itemConfig;
        private SpawnerConfig _spawnerConfig;

        private GameObject _itemPrefab;
        private SpawnContainerView _spawnContainerView;

        private CancellationTokenSource _cancellationTokenSource;

        private float _currentCount;

        private List<GameObject> _items;

        public void Init()
        {
            _currentCount = 0;
            _items = new List<GameObject>();

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

            foreach (var entity in _droppableFilter)
            {
                ref var droppable = ref _droppableFilter.Get2(entity);

                if (droppable.IsReadyToDrop)
                {
                    OnItemRemoved(_droppableFilter.GetEntity(entity));

                    droppable.IsReadyToDrop = false;
                    droppable.IsDropped = true;
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
        }

        private GameObject SpawnItem(bool isPositionInitialized = false, 
            EcsEntity itemEntity = default(EcsEntity))
        {
            GameObject spawnedItem = _items.FirstOrDefault(
                item => item.activeSelf == false);

            if (spawnedItem == null)
            {
                spawnedItem = CreateItem(isPositionInitialized, itemEntity);
            }
            else
            {
                spawnedItem.SetActive(true);

                if (itemEntity == default(EcsEntity))
                {
                    CreateItemEntity(spawnedItem, isPositionInitialized);
                }
            }

            spawnedItem.GetComponent<Rigidbody>().velocity = Vector3.zero;

            return spawnedItem;
        }

        private void OnItemRemoved(EcsEntity itemEntity)
        {
            GameObject spawnedItem;

            spawnedItem = SpawnItem(true, itemEntity);

            ref var model = ref itemEntity.Get<ModelComponent>();
            ref var spawnable = ref itemEntity.Get<SpawnableComponent>();
            ref var droppable = ref itemEntity.Get<DroppableComponent>();

            model.Transform = spawnedItem.transform;
            spawnedItem.GetComponent<ItemView>().Initialize(spawnable.Guid);
            droppable.IsDropped = true;
        }

        private GameObject CreateItem(bool isPositionInitialized = false, EcsEntity itemEntity = default(EcsEntity))
        {
            GameObject item = UnityEngine.Object.Instantiate(_itemPrefab);
            item.transform.parent = _spawnContainerView.transform;
            
            if (itemEntity == default(EcsEntity))
            {
                itemEntity = CreateItemEntity(item, isPositionInitialized);
            }

            _items.Add(item);

            return item;
        }

        private EcsEntity CreateItemEntity(GameObject item, bool isPositionInitialized = false)
        {
            EcsEntity itemEntity = _ecsWorld.NewEntity();
            ItemView itemView = item.GetComponent<ItemView>();

            ref var itemTag = ref itemEntity.Get<ItemTag>();
            ref var model = ref itemEntity.Get<ModelComponent>();
            ref var droppable = ref itemEntity.Get<DroppableComponent>();
            ref var spawnable = ref itemEntity.Get<SpawnableComponent>();
            ref var obtainable = ref itemEntity.Get<ObtainableComponent>();
            ref var removable = ref itemEntity.Get<RemovableComponent>();

            model.Transform = item.transform;
            droppable.IsDropped = false;
            droppable.IsReadyToDrop = false;
            spawnable.Guid = itemView.Guid;
            spawnable.IsPositionRandomized = isPositionInitialized;
            obtainable.InventoryIcon = itemView.Icon;
            obtainable.IsObtained = false;
            removable.IsRemoved = false;

            return itemEntity;
        }

        private async UniTask LoadPrefab(CustomAssetReferenceTo<GameObject> prefab, CancellationToken cancellationToken)
        {
            _itemPrefab = await Addressables.LoadAssetAsync<GameObject>(prefab)
                .ToUniTask(cancellationToken: cancellationToken);

            _cancellationTokenSource = null;
        }
    }
}