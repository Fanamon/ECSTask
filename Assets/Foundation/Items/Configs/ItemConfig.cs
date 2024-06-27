using Foundation.Shared;
using UnityEngine;

namespace Foundation.Items.Configs
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Foundation/StaticData/ItemConfig", order = 2)]
    public class ItemConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _dropPower;

        [field: SerializeField] public CustomAssetReferenceTo<GameObject> Prefab;

        public float DropPower => _dropPower;
    }
}