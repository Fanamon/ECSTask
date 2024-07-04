using Cysharp.Threading.Tasks;
using Foundation.Inventory.Components;
using Foundation.Items.Views;
using Foundation.Movement.Components;
using Foundation.Player.Components;
using Foundation.Player.Configs;
using Foundation.Player.Tags;
using Foundation.Player.Views;
using Foundation.Shared;
using Foundation.SpawnSystem.Views.SpawnPoints;
using Leopotam.Ecs;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

namespace Foundation.Player.Systems
{
    sealed class PlayerInitializeSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private EcsWorld _ecsWorld = null;
        private PlayerConfig _config;
        private PlayerSpawnPoint _spawnPoint;
        private CameraFollowInitializer _cameraFollowInitializer;

        private EcsEntity _playerEntity;

        private CancellationTokenSource _cancellationTokenSource;

        public void Init()
        {
            _playerEntity = _ecsWorld.NewEntity();

            ref var playerTag = ref _playerEntity.Get<PlayerTag>();
            ref var direction = ref _playerEntity.Get<DirectionComponent>();

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

        private async UniTask LoadPrefab(CustomAssetReferenceTo<GameObject> prefab, CancellationToken cancellationToken)
        {
            GameObject playerPrefab =
                await Addressables.LoadAssetAsync<GameObject>(prefab)
                .ToUniTask(cancellationToken: cancellationToken);

            if (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested == false)
            {
                CreatePlayer(playerPrefab);
            }
        }

        private void CreatePlayer(GameObject playerPrefab)
        {
            GameObject player = UnityEngine.Object.
                Instantiate(playerPrefab, _spawnPoint.SpawnPoint.position, Quaternion.identity);

            ref var model = ref _playerEntity.Get<ModelComponent>();
            ref var movable = ref _playerEntity.Get<MovableComponent>();
            ref var dampingDirection = ref _playerEntity.Get<DampingDirectionComponent>();
            ref var stackKeep = ref _playerEntity.Get<StackKeepComponent>();
            ref var animator = ref _playerEntity.Get<AnimatorComponent>();

            model.Transform = player.transform;
            movable.NavMeshAgent = player.GetComponent<NavMeshAgent>();
            dampingDirection.Duration = _config.DampingDuration;
            stackKeep.ItemObtainerView = player.GetComponent<ItemObtainerView>();
            stackKeep.IsObtainerSystemSubscribed = false;
            stackKeep.ItemGuids = new Stack<Guid>();
            animator.Animator = player.GetComponentInChildren<Animator>();
            animator.Damping = _config.DampAnimationDuration;

            _cameraFollowInitializer.SetCameraFollow(model.Transform);
        }
    }
}