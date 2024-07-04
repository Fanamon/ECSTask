using Foundation.Shared;
using UnityEngine;

namespace Foundation.Inventory.Configs
{
    [CreateAssetMenu(fileName = "ItemInventoryConfig", 
        menuName = "Foundation/StaticData/ItemInventoryConfig", order = 4)]
    public class ItemInventoryConfig : ScriptableObject
    {
        [SerializeField] private CustomAssetReferenceTo<GameObject> _itemViewPrefab;

        public CustomAssetReferenceTo<GameObject> ItemViewPrefab => _itemViewPrefab;
    }
}