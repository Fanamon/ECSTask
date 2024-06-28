using Foundation.Inventory.Components;
using Foundation.Items.Tags;
using Foundation.Items.Views;
using Foundation.Movement.Components;
using Foundation.Player.Tags;
using Foundation.Player.Views;
using Foundation.SpawnSystem.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace Foundation.Player.Systems
{
    public class PlayerItemsObtainerSystem : IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsFilter<PlayerTag, StackKeepComponent> _obtainerFilter = null;
        private readonly EcsFilter<ItemTag, ModelComponent, 
            SpawnableComponent> _itemFilter = null;

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
        }

        public void Destroy()
        {
            foreach (var entity in _obtainerFilter)
            {
                ref var stackKeep = ref _obtainerFilter.Get2(entity);

                stackKeep.ItemObtainerView.ItemObtained -= OnItemObtained;
            }
        }

        private void OnItemObtained(ItemObtainerView player, ItemView item)
        {
            foreach (var entity in _itemFilter)
            {
                ref var model = ref _itemFilter.Get2(entity);
                ref var spawnable = ref _itemFilter.Get3(entity);

                if (spawnable.Guid == item.Guid)
                {
                    model.Transform.gameObject.SetActive(false);
                }
            }

            foreach (var entity in _obtainerFilter)
            {
                ref var stackKeep = ref _obtainerFilter.Get2(entity);

                if (stackKeep.Guid == player.Guid)
                {
                    stackKeep.Items.Push(item);
                    Debug.Log(stackKeep.Items.Count);
                }
            }
        }
    }
}