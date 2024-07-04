using Foundation.Items.Views;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Foundation.Player.Views
{
    public class ItemObtainerView : MonoBehaviour
    {
        [SerializeField] private Transform _dropPoint;

        private Transform _transform;

        public event UnityAction<ItemObtainerView, ItemView> ItemObtained;

        public Guid Guid { get; private set; }
        public Vector3 DropPoint => GetRandomDropPoint();

        private void Awake()
        {
            _transform = transform;
            Guid = Guid.NewGuid();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ItemView itemView))
            {
                ItemObtained?.Invoke(this, itemView);
            }
        }

        private Vector3 GetRandomDropPoint()
        {
            float radius = (_dropPoint.position - transform.position).magnitude;

            Vector2 centerPoint = new Vector2(_transform.position.x, _transform.position.z);
            Vector2 randomPoint = centerPoint + UnityEngine.Random.insideUnitCircle.normalized * radius;

            return new Vector3(randomPoint.x, _dropPoint.position.y, randomPoint.y);
        }
    }
}