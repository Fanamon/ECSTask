using Foundation.Player.Views;
using System;
using System.Collections.Generic;

namespace Foundation.Inventory.Components
{
    internal struct StackKeepComponent
    {
        public ItemObtainerView ItemObtainerView;

        public bool IsObtainerSystemSubscribed;

        public Stack<Guid> ItemGuids;
    }
}