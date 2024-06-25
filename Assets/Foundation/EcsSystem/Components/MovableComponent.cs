using System;
using UnityEngine.AI;

namespace Foundation.EcsSystem.Components
{
    [Serializable]
    public struct MovableComponent
    {
        public NavMeshAgent NavMeshAgent;
    }
}