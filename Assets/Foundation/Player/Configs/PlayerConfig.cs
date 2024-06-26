using Foundation.Shared;
using UnityEngine;

namespace Foundation.Player.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Foundation/StaticData/PlayerConfig", order = 1)]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _dampingDuration = 0.35f;

        [field: SerializeField] public CustomAssetReferenceTo<GameObject> Prefab;

        public float DampingDuration => _dampingDuration;
    }
}