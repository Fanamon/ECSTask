using System;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.GUI.Views
{
    public class ItemInventoryView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public Guid Guid { get; private set; }

        public void Initialize(Guid guid, Sprite sprite)
        {
            Guid = guid;
            _icon.sprite = sprite;
        }

        private void OnDisable()
        {
            Guid = Guid.Empty;
        }
    }
}