using System;
using UnityEngine;

namespace Foundation.Items.Views
{
    public class ItemView : MonoBehaviour
    {
        public Guid Guid { get; private set; }

        private void Awake()
        {
            Guid guid = Guid.NewGuid();
        }
    }
}