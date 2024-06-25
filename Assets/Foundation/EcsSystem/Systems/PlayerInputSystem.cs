using Foundation.Controller.Interactions;
using Foundation.EcsSystem.Components;
using Foundation.EcsSystem.Tags;
using Leopotam.Ecs;
using UnityEngine;

namespace Foundation.EcsSystem.Systems
{
    sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<PlayerTag, DirectionComponent> _directionFilter = null;
        private PlayerInputAction _playerInputAction;

        public void Init()
        {
            _playerInputAction = new PlayerInputAction();
            _playerInputAction.Enable();
        }

        public void Run()
        {
            foreach (var index in _directionFilter)
            {
                ref var directionComponent = ref _directionFilter.Get2(index);
                ref var direction = ref directionComponent.Direction;

                direction = _playerInputAction.Player.Move.ReadValue<Vector2>();
            }
        }

        public void Destroy()
        {
            _playerInputAction.Disable();
        }
    }
}