using Foundation.Items.Views;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Foundation.Player.Views
{
    public class ItemObtainerView : MonoBehaviour
    {
        [SerializeField] private Transform _dropPoint;

        public event UnityAction<ItemObtainerView, ItemView> ItemObtained;

        public Guid Guid { get; private set; }

        public Vector3 DropPoint => _dropPoint.position - transform.position;

        private void Awake()
        {
            Guid = Guid.NewGuid();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ItemView itemView))
            {
                ItemObtained?.Invoke(this, itemView);
            }
        }
    }
}