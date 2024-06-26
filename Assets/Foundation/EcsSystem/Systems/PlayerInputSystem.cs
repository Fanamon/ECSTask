using Foundation.Controller.Interactions;
using Foundation.EcsSystem.Components;
using Foundation.EcsSystem.Tags;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Foundation.EcsSystem.Systems
{
    sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<PlayerTag, DirectionComponent, 
            DampingDirectionComponent> _directionFilter = null;
        private PlayerInputAction _playerInputAction;

        private float _damping;

        private Vector2 _startControllerPosition;
        private Vector2 _moveDirection;

        private Tweener _dampingTweener;

        public void Init()
        {
            InitializeDamping();

            _playerInputAction = new PlayerInputAction();
            _playerInputAction.Enable();

            _playerInputAction.Player.Tap.performed += OnTapped;
        }

        public void Run()
        {
            foreach (var index in _directionFilter)
            {
                ref var directionComponent = ref _directionFilter.Get2(index);
                ref var direction = ref directionComponent.Direction;

                direction = _moveDirection.normalized;
            }
        }

        public void Destroy()
        {
            if (_dampingTweener.IsActive())
            {
                _dampingTweener.Kill();
            }

            _playerInputAction.Disable();
        }

        private void InitializeDamping()
        {
            foreach (var index in _directionFilter)
            {
                ref var damping = ref _directionFilter.Get3(index);

                _damping = damping.Duration;
            }
        }

        private void OnTapped(InputAction.CallbackContext context)
        {
            if (_dampingTweener.IsActive())
            {
                _dampingTweener.Kill();
            }

            _startControllerPosition = _playerInputAction.Player.Position.ReadValue<Vector2>();

            _playerInputAction.Player.Swipe.performed += OnSwiped;
            _playerInputAction.Player.Tap.canceled += OnTapCanceled;
        }

        private void OnSwiped(InputAction.CallbackContext context)
        {
            _moveDirection = _playerInputAction.Player.Position.ReadValue<Vector2>() - 
                _startControllerPosition;
        }

        private void OnTapCanceled(InputAction.CallbackContext context)
        {
            _playerInputAction.Player.Swipe.performed -= OnSwiped;
            _playerInputAction.Player.Tap.canceled -= OnTapCanceled;

            _dampingTweener = DOVirtual.Vector2(_moveDirection, Vector2.zero, 
                _damping, value => _moveDirection = value);
        }
    }
}