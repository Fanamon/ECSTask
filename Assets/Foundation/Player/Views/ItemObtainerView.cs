using Foundation.Items.Views;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Foundation.Player.Views
{
    public class ItemObtainerView : MonoBehaviour
    {
        public event UnityAction<ItemObtainerView, ItemView> ItemObtained;

        public Guid Guid { get; private set; }

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