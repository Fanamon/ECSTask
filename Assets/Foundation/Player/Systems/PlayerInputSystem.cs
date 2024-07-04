using Cysharp.Threading.Tasks;
using DG.Tweening;
using Foundation.Controller.Interactions;
using Foundation.Movement.Components;
using Foundation.Player.Components;
using Foundation.Player.ImportedAssets.Stickman.Scripts;
using Foundation.Player.Tags;
using Leopotam.Ecs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Foundation.Player.Systems
{
    sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<PlayerTag, DirectionComponent, 
            DampingDirectionComponent> _directionFilter = null;
        private readonly EcsFilter<PlayerTag, AnimatorComponent>
            _animatorFilter = null;
        private PlayerInputAction _playerInputAction;

        private Vector2 _startControllerPosition;
        private Vector2 _moveDirection;

        private bool _isAnimationStarted;

        private List<Tweener> _dampingTweeners;
        private List<Tweener> _dampingAnimationTweeners;
        private List<CancellationTokenSource> _cancellationTokenSources;

        public void Init()
        {
            _dampingTweeners = new List<Tweener>();
            _dampingAnimationTweeners = new List<Tweener>();
            _cancellationTokenSources = new List<CancellationTokenSource>();

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
            ResetAnimationTweenersAndTokens();

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
            ResetAnimationTweenersAndTokens();

            _startControllerPosition = _playerInputAction.Player.Position.ReadValue<Vector2>();

            _playerInputAction.Player.Swipe.performed += OnSwiped;
            _playerInputAction.Player.Tap.canceled += OnTapCanceled;
        }

        private void OnSwiped(InputAction.CallbackContext context)
        {
            if (_isAnimationStarted == false)
            {
                foreach (var entity in _animatorFilter)
                {
                    ref var animatorComponent = ref _animatorFilter.Get2(entity);
                    Animator animator = animatorComponent.Animator;
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                    AnimateRunning(animator, cancellationTokenSource.Token).Forget();

                    CancellationTokenSource canceledTokenSource =
                        _cancellationTokenSources.FirstOrDefault(tokenSource => tokenSource.IsCancellationRequested);

                    _cancellationTokenSources.Add(cancellationTokenSource);
                }

                _isAnimationStarted = true;
            }

            _moveDirection = _playerInputAction.Player.Position.ReadValue<Vector2>() - 
                _startControllerPosition;
        }

        private void OnTapCanceled(InputAction.CallbackContext context)
        {
            ResetAnimationTweenersAndTokens();

            _playerInputAction.Player.Swipe.performed -= OnSwiped;
            _playerInputAction.Player.Tap.canceled -= OnTapCanceled;

            foreach (var entity in _directionFilter)
            {
                ref var damping = ref _directionFilter.Get3(entity);

                Tweener dampingTweener = DOVirtual.Vector2(_moveDirection, Vector2.zero,
                damping.Duration, value => _moveDirection = value);

                _dampingTweeners.Add(dampingTweener);
            }

            foreach (var entity in _animatorFilter)
            {
                ref var animatorComponent = ref _animatorFilter.Get2(entity);
                Animator animator = animatorComponent.Animator;

                if (animator != null)
                {
                    float currentSpeed = animator.GetFloat(Animator.StringToHash(StickmanAnimatorParameters.Speed));

                    Tweener dampingAnimatorSpeed = DOVirtual.Float(currentSpeed, StickmanAnimatorParameters.MinMoveSpeed,
                        animatorComponent.Damping, value => animator?.SetFloat(StickmanAnimatorParameters.Speed, value));
                    Tweener killedTweener = _dampingAnimationTweeners.FirstOrDefault(tweener =>
                    tweener.IsActive() == false);

                    _dampingAnimationTweeners.Add(dampingAnimatorSpeed);
                }
            }
        }

        private async UniTask AnimateRunning(Animator animator, 
            CancellationToken cancellationToken)
        {
            float currentSpeed = animator.GetFloat(Animator.StringToHash(StickmanAnimatorParameters.Speed));
            float currentTimer = 0f;

            while (cancellationToken.IsCancellationRequested == false && animator != null)
            {
                if (animator != null)
                {
                    currentTimer += Time.deltaTime;
                    currentSpeed = Mathf.Lerp(currentSpeed, StickmanAnimatorParameters.MaxMoveSpeed,
                        currentTimer / StickmanAnimatorParameters.AccelerateTimeInSeconds);
                    animator?.SetFloat(StickmanAnimatorParameters.Speed, currentSpeed);

                    await UniTask.Yield(cancellationToken);
                }
            }
        }

        private void ResetAnimationTweenersAndTokens()
        {
            foreach (var tweener in _dampingAnimationTweeners)
            {
                if (tweener.IsActive())
                {
                    tweener.Kill();
                }
            }

            foreach (var tokenSource in _cancellationTokenSources)
            {
                if (tokenSource.IsCancellationRequested == false)
                {
                    tokenSource?.Cancel();
                    tokenSource?.Dispose();
                }
            }

            _isAnimationStarted = false;
        }
    }
}