using Foundation.Movement.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace Foundation.Movement.Systems
{
    sealed class MovementSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ModelComponent, MovableComponent, 
            DirectionComponent> _movableFilter = null;

        public void Run()
        {
            foreach (var entity in _movableFilter)
            {
                ref var modelComponent = ref _movableFilter.Get1(entity);
                ref var movableComponent = ref _movableFilter.Get2(entity);
                ref var directionComponent = ref _movableFilter.Get3(entity);

                ref var direction = ref directionComponent.Direction;
                ref var transform = ref modelComponent.ModelTransform;

                ref var navMeshAgent = ref movableComponent.NavMeshAgent;

                var endMovementPoint = new Vector3(transform.position.x + direction.x, 
                    transform.position.y, transform.position.z + direction.y);

                navMeshAgent.SetDestination(endMovementPoint);
            }
        }
    }
}