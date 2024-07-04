using System;
using UnityEngine;

namespace Foundation.Movement.Components
{
    internal struct DroppableComponent
    {
        public Vector3 DropPoint;

        public bool IsDropped;

        public bool IsReadyToDrop;
    }
}