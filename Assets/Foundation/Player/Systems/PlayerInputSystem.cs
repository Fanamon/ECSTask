using DG.Tweening;
using Foundation.Controller.Interactions;
using Foundation.Movement.Components;
using Foundation.Player.Tags;
using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Foundation.Player.Systems
{
    sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<PlayerTag, DirectionComponent, 
            DampingDirectionComponent> _directionFilter = null;
        private PlayerInputAction _playerInputAction;

        private Vector2 _startControllerPosition;
        private Vector2 _moveDirection;

        private List<Tweener> _dampingTweeners;
        public void Init()
        {
            _dampingTweeners = new List<Tweener>();

            _playerInputAction = new PlayerInputAction();
            _playerInputAction.Enable();

            _playerInputAction.Player.Tap.performed += OnTapped;
        }

        public void Run()
        {
            foreach (var entity in _directionFilter)
            {
                ref var directionComponent = ref _directionFilter.Get2(entity);
                ref var direction = ref directionComponent.Direction;

                direction = new Vector3(_moveDirection.x, 0, _moveDirection.y).normalized;
            }
        }

        public void Destroy()
        {
            foreach (var dampingTweener in _dampingTweeners)
            {
                if (dampingTweener.IsActive())
                {
                    dampingTweener.Kill();
                }
            }

            _playerInputAction.Disable();
        }

        private void OnTapped(InputAction.CallbackContext context)
        {
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

            foreach (var entity in _directionFilter)
            {
                ref var damping = ref _directionFilter.Get3(entity);

                Tweener dampingTweener = DOVirtual.Vector2(_moveDirection, Vector2.zero,
                damping.Duration, value => _moveDirection = value);
                _dampingTweeners.Add(dampingTweener);
            }
        }
    }
}