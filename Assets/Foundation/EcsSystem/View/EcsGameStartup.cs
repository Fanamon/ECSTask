using Foundation.Items.Configs;
using Foundation.Movement.Systems;
using Foundation.Player.Configs;
using Foundation.Player.Systems;
using Foundation.Player.Views;
using Foundation.SpawnSystem.Configs;
using Foundation.SpawnSystem.Systems;
using Foundation.SpawnSystem.Views.SpawnPoints;
using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

namespace Foundation.EcsSystem.View
{
    public sealed class EcsGameStartup : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private ItemConfig _itemConfig;
        [SerializeField] private SpawnerConfig _spawnerConfig;

        [SerializeField] private PlayerSpawnPoint _playerSpawnPoint;
        [SerializeField] private ItemsCircleSpawnerPointsData _itemsCircleSpawnerPointsData;
        [SerializeField] private CameraFollowInitializer _cameraFollowInitializer;

        private EcsWorld _world;
        private EcsSystems _updateSystems;
        private EcsSystems _lateUpdateSystems;

        private void Start()
        {
            _world = new EcsWorld();
            _updateSystems = new EcsSystems(_world);
            _lateUpdateSystems = new EcsSystems(_world);

            _updateSystems.ConvertScene();
            _lateUpdateSystems.ConvertScene();

            AddInjections();
            AddOneFrames();
            AddSystems();

            _updateSystems.Init();
            _lateUpdateSystems.Init();
        }

        private void Update()
        {
            _updateSystems?.Run();
        }

        private void LateUpdate()
        {
            _lateUpdateSystems?.Run();
        }

        private void OnDestroy()
        {
            if (_updateSystems != null)
            {
                _updateSystems.Destroy();
                _lateUpdateSystems.Destroy();
                _world.Destroy();

                _updateSystems = null;
                _lateUpdateSystems = null;
                _world = null;
            }
        }

        private void AddInjections()
        {
            _updateSystems.
                Inject(_playerConfig).
                Inject(_playerSpawnPoint).
                Inject(_cameraFollowInitializer).
                Inject(_itemConfig).
                Inject(_itemsCircleSpawnerPointsData).
                Inject(_spawnerConfig);
        }

        private void AddSystems()
        {
            _updateSystems.
                Add(new PlayerInitializeSystem()).
                Add(new PlayerInputSystem()).
                Add(new ItemsSpawnSystem()).
                Add(new ItemsCirclePositionerSystem());

            _lateUpdateSystems.
                Add(new MovementSystem());
        }

        private void AddOneFrames()
        {

        }
    }
}