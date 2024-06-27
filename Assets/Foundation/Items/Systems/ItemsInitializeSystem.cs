using Cysharp.Threading.Tasks;
using Foundation.Items.Configs;
using Foundation.Items.Tags;
using Foundation.Movement.Components;
using Foundation.Shared;
using Leopotam.Ecs;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Foundation.Items.Systems
{
    public class ItemsInitializeSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private EcsWorld _ecsWorld = null;
        private ItemConfig _config;

        private GameObject _itemPrefab;

        private CancellationTokenSource _cancellationTokenSource;

        public void Init()
        {
            if (_cancellationTokenSource != null &&
                _cancellationTokenSource.IsCancellationRequested == false)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            LoadPrefab(_config.Prefab, _cancellationTokenSource.Token).Forget();
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

        public GameObject CreateItem()
        {
            GameObject item = Object.Instantiate(_itemPrefab);

            CreateItemEntity(item);

            return item;
        }

        private void CreateItemEntity(GameObject item)
        {
            EcsEntity itemEntity = _ecsWorld.NewEntity();

            ref var itemTag = ref itemEntity.Get<ItemTag>();
            ref var model = ref itemEntity.Get<ModelComponent>();
            ref var direction = ref itemEntity.Get<DirectionComponent>();
            ref var droppable = ref itemEntity.Get<DroppableComponent>();

            model.Transform = item.transform;
            droppable.DropPower = _config.DropPower;
            droppable.Rigidbody = item.GetComponent<Rigidbody>();
        }

        private async UniTask LoadPrefab(CustomAssetReferenceTo<GameObject> prefab, CancellationToken cancellationToken)
        {
            _itemPrefab = await Addressables.LoadAssetAsync<GameObject>(prefab)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}