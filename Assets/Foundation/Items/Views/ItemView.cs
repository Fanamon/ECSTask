using System;
using UnityEngine;

namespace Foundation.Items.Views
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Sprite _icon;

        private bool _isGuidInitialized = false;

        public Sprite Icon => _icon;
        public Guid Guid { get; private set; }

        public void Initialize(Guid guid)
        {
            _isGuidInitialized = true;

            Guid = guid;
        }

        private void OnEnable()
        {
            if (_isGuidInitialized == false)
            {
                Guid = Guid.NewGuid();
            }
        }

        private void OnDisable()
        {
            Guid = Guid.Empty;
            _isGuidInitialized = false;
        }
    }
}