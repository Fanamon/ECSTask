using UnityEngine;

namespace Foundation.SpawnSystem.Views.SpawnPoints
{
    public class ItemsCircleSpawnerPointsData : MonoBehaviour
    {
        [SerializeField] private Transform _centerPoint;
        [SerializeField] private Transform _handlerPoint;

        public Transform CenterPoint => _centerPoint;
        public Transform HandlerPoint => _handlerPoint;
    }
}