using Foundation.Items.Views;
using Foundation.Player.Views;
using System;
using System.Collections.Generic;

namespace Foundation.Inventory.Components
{
    internal struct StackKeepComponent
    {
        public Guid Guid;

        public ItemObtainerView ItemObtainerView;

        public bool IsObtainerSystemSubscribed;

        public Stack<ItemView> Items;
    }
}