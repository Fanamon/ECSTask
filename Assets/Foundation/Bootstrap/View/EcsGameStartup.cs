using Foundation.EcsSystem.Systems;
using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

namespace Foundation.Bootstrap.View
{
    public sealed class EcsGameStartup : MonoBehaviour
    {
        private EcsWorld _world;
        private EcsSystems _systems;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            _systems.ConvertScene();

            AddInjections();
            AddOneFrames();
            AddSystems();

            _systems.Init();
        }

        private void Update()
        {
            _systems.Run();
        }

        private void AddInjections()
        {

        }

        private void AddSystems()
        {
            _systems.
                Add(new PlayerInputSystem()).
                Add(new MovementSystem());
        }

        private void AddOneFrames()
        {

        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _world.Destroy();

                _systems = null;
                _world = null;
            }
        }
    }
}