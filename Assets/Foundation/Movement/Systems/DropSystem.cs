using Foundation.Items.Tags;
using Foundation.Movement.Components;
using Leopotam.Ecs;

namespace Foundation.Movement.Systems
{
    sealed class DropSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ItemTag, ModelComponent,
            DroppableComponent> _droppableFilter = null;

        public void Run()
        {
            foreach (var entity in _droppableFilter)
            {
                ref var droppable = ref _droppableFilter.Get3(entity);

                if (droppable.IsDropped)
                {
                    ref var model = ref _droppableFilter.Get2(entity);

                    model.Transform.position = droppable.DropPoint;
                    droppable.IsDropped = false;
                }
            }
        }
    }
}