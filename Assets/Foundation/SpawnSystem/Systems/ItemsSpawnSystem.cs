using Cysharp.Threading.Tasks;
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

        private void SpawnItem()
        {
            GameObject disabledItem = _items.FirstOrDefault(
                item => item.activeSelf == false);

            if (disabledItem == null)
            {
                disabledItem = CreateItem();
            }
            else
            {
                disabledItem.SetActive(true);
            }
        }

        private GameObject CreateItem()
        {
            GameObject item = Object.Instantiate(_itemPrefab);
            item.transform.parent = _spawnContainerView.transform;

            CreateItemEntity(item);

            return item;
        }

        private void CreateItemEntity(GameObject item)
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
            spawnable.IsPositionRandomized = false;
        }

        private async UniTask LoadPrefab(CustomAssetReferenceTo<GameObject> prefab, CancellationToken cancellationToken)
        {
            _itemPrefab = await Addressables.LoadAssetAsync<GameObject>(prefab)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}