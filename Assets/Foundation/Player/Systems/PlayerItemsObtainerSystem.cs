using Foundation.Inventory.Components;
using Foundation.Items.Components;
using Foundation.Items.Tags;
using Foundation.Items.Views;
using Foundation.Movement.Components;
using Foundation.Player.Tags;
using Foundation.Player.Views;
using Foundation.SpawnSystem.Components;
using Leopotam.Ecs;

namespace Foundation.Player.Systems
{
    public class PlayerItemsObtainerSystem : IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<PlayerTag, StackKeepComponent> _obtainerFilter = null;
        private readonly EcsFilter<ItemTag, RemovableComponent, 
            SpawnableComponent, DroppableComponent> _removableFilter = null;
        private readonly EcsFilter<ItemTag, ModelComponent, 
            SpawnableComponent, ObtainableComponent> _itemFilter = null;

        public void Run()
        {
            foreach (var entity in _obtainerFilter)
            {
                ref var stackKeep = ref _obtainerFilter.Get2(entity);

                if (stackKeep.IsObtainerSystemSubscribed == false)
                {
                    stackKeep.IsObtainerSystemSubscribed = true;
                    stackKeep.ItemObtainerView.ItemObtained += OnItemObtained;
                }
            }

            TryDropItem();
        }

        public void Destroy()
        {
            foreach (var entity in _obtainerFilter)
            {
                ref var stackKeep = ref _obtainerFilter.Get2(entity);

                stackKeep.IsObtainerSystemSubscribed = false;
                stackKeep.ItemObtainerView.ItemObtained -= OnItemObtained;
            }
        }

        private void OnItemObtained(ItemObtainerView player, ItemView item)
        {
            foreach (var entity in _itemFilter)
            {
                ref var spawnable = ref _itemFilter.Get3(entity);

                if (spawnable.Guid == item.Guid)
                {
                    ref var model = ref _itemFilter.Get2(entity);
                    ref var obtainable = ref _itemFilter.Get4(entity);

                    model.Transform.gameObject.SetActive(false);
                    obtainable.IsObtained = true;
                }
            }

            foreach (var entity in _obtainerFilter)
            {
                ref var stackKeep = ref _obtainerFilter.Get2(entity);

                if (stackKeep.ItemObtainerView.Guid == player.Guid)
                {
                    stackKeep.ItemGuids.Push(item.Guid);
                }
            }
        }

        private void TryDropItem()
        {
            foreach (var entity in _removableFilter)
            {
                ref var removable = ref _removableFilter.Get2(entity);

                if (removable.IsRemoved)
                {
                    foreach (var playerEntiry in _obtainerFilter)
                    {
                        ref var stackable = ref _obtainerFilter.Get2(entity);
                        ref var spawnable = ref _removableFilter.Get3(entity);

                        if (stackable.ItemGuids.Contains(spawnable.Guid))
                        {
                            ref var droppable = ref _removableFilter.Get4(entity);

                            stackable.ItemGuids.Pop();
                            removable.IsRemoved = false;
                            droppable.IsReadyToDrop = true;
                            droppable.DropPoint = stackable.ItemObtainerView.DropPoint;
                        }
                    }
                }
            }
        }
    }
}