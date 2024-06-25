using Cysharp.Threading.Tasks;
using Foundation.Controller.Interactions;
using Foundation.EcsSystem.Components;
using Foundation.EcsSystem.Tags;
using Leopotam.Ecs;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Foundation.EcsSystem.Systems
{
    sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private const float MinMoveDampen = 0.1f;

        private readonly EcsFilter<PlayerTag, DampingMovementComponent, 
            DirectionComponent> _directionFilter = null;
        private PlayerInputAction _playerInputAction;

        private float _damping;

        private Vector2 _destination;
        private CancellationTokenSource _dampingCancellationTokenSource;

        public void Init()
        {
            InitializeDamping();

            _playerInputAction = new PlayerInputAction();
            _playerInputAction.Enable();

            _playerInputAction.Player.Move.performed += OnMoved;
            _playerInputAction.Player.Move.canceled += OnMoveCanceled;
        }

        public void Run()
        {
            foreach (var index in _directionFilter)
            {
                ref var directionComponent = ref _directionFilter.Get3(index);
                ref var direction = ref directionComponent.Direction;

                direction = _destination;
            }
        }

        public void Destroy()
        {
            _dampingCancellationTokenSource?.Cancel();
            _dampingCancellationTokenSource?.Dispose();
            _dampingCancellationTokenSource = null;

            _playerInputAction.Player.Move.performed -= OnMoved;
            _playerInputAction.Player.Move.canceled -= OnMoveCanceled;

            _playerInputAction.Disable();
        }

        private void OnMoved(InputAction.CallbackContext context)
        {
            if (_dampingCancellationTokenSource != null)
            {
                _dampingCancellationTokenSource?.Cancel();
                _dampingCancellationTokenSource?.Dispose();
                _dampingCancellationTokenSource = null;
            }

            _destination = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _dampingCancellationTokenSource = new CancellationTokenSource();

            Damp(_dampingCancellationTokenSource.Token).Forget();
        }

        private void InitializeDamping()
        {
            foreach (var index in _directionFilter)
            {
                ref var damping = ref _directionFilter.Get2(index).Damping;

                _damping = damping;
            }
        }

        private async UniTask Damp(CancellationToken cancellationToken)
        {
            while (Mathf.Abs(_destination.magnitude) > MinMoveDampen)
            {
                _destination = new Vector2(DampValue(_destination.x), DampValue(_destination.y));

                await UniTask.Yield(cancellationToken).SuppressCancellationThrow();
            }

            _destination = Vector2.zero;
            _dampingCancellationTokenSource = null;
        }

        private float DampValue(float value)
        {
            value *= Mathf.Pow(_damping, Time.deltaTime);

            return value;
        }
    }
}