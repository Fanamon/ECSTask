using Foundation.Items.Tags;
using Foundation.Movement.Components;
using Foundation.Shared;
using Foundation.SpawnSystem.Components;
using Foundation.SpawnSystem.Configs;
using Foundation.SpawnSystem.Views.SpawnPoints;
using Leopotam.Ecs;
using UnityEngine;

namespace Foundation.SpawnSystem.Systems
{
    sealed class ItemsCirclePositionerSystem : IEcsRunSystem
    {
        private const float MaxRayDistance = 0f;

        private SpawnerConfig _config;
        private ItemsCircleSpawnerPointsData _pointsData;

        private readonly EcsFilter<ItemTag, ModelComponent, 
            SpawnableComponent> _positionerFilter = null;

        public void Run()
        {
            foreach (var entity in _positionerFilter)
            {
                ref var model = ref _positionerFilter.Get2(entity);
                ref var spawnable = ref _positionerFilter.Get3(entity);

                if (model.Transform.gameObject.activeSelf && spawnable.IsPositionRandomized == false)
                {
                    SetPosition(model.Transform);

                    spawnable.IsPositionRandomized = true;
                }
            }
        }

        private void SetPosition(Transform item)
        {
            Vector3 randomPoint;

            do
            {
                randomPoint = GetRandomPoint();
            }
            while (Physics.SphereCast(randomPoint, _config.AntispawnRadius, 
            Vector3.zero, out RaycastHit hit, MaxRayDistance, LayerMasks.AntispawnLayer));

            item.position = randomPoint;
            item.rotation = Quaternion.identity;
        }

        private Vector3 GetRandomPoint()
        {
            float radius = (_pointsData.HandlerPoint.position - 
                _pointsData.CenterPoint.position).magnitude;
            Vector2 centerPoint = new Vector2(_pointsData.CenterPoint.position.x,
                _pointsData.CenterPoint.position.z);

            Vector2 randomPoint = centerPoint + Random.insideUnitCircle * radius;

            return new Vector3(randomPoint.x, _pointsData.CenterPoint.position.y, randomPoint.y);
        }
    }
}