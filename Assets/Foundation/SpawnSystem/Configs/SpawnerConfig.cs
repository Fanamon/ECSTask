using UnityEngine;

namespace Foundation.SpawnSystem.Configs
{
    [CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Foundation/StaticData/SpawnerConfig", order = 3)]
    public class SpawnerConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _delay;
        [SerializeField, Min(0f)] private float _antispawnRadius;

        public float Delay => _delay;
        public float AntispawnRadius => _antispawnRadius;
    }
}