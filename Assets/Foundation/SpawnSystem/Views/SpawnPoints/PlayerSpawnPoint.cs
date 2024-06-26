using UnityEngine;

namespace Foundation.SpawnSystem.Views.SpawnPoints
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public Transform SpawnPoint => _spawnPoint;
    }
}