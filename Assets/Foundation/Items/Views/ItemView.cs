using System;
using UnityEngine;

namespace Foundation.Items.Views
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Sprite _icon;

        private bool _isGuidInitialized;

        public Sprite Icon => _icon;
        public Guid Guid { get; private set; }

        public void Initialize(Guid guid)
        {
            Guid = guid;
            _isGuidInitialized = true;
        }

        private void OnEnable()
        {
            if (_isGuidInitialized == false)
            {
                Guid = Guid.NewGuid();
                _isGuidInitialized = true;
            }
        }

        private void OnDisable()
        {
            _isGuidInitialized = false;
        }
    }
}