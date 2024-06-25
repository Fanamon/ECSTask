using Foundation.EcsSystem.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace Foundation.EcsSystem.Systems
{
    sealed class MovementSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ModelComponent, MovableComponent, 
            DirectionComponent> _movableFilter = null;

        public void Run()
        {
            foreach (var index in _movableFilter)
            {
                ref var modelComponent = ref _movableFilter.Get1(index);
                ref var movableComponent = ref _movableFilter.Get2(index);
                ref var directionComponent = ref _movableFilter.Get3(index);

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