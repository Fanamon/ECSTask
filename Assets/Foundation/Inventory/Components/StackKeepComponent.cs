using Foundation.Items.Views;
using Foundation.Player.Views;
using System.Collections.Generic;

namespace Foundation.Inventory.Components
{
    internal struct StackKeepComponent
    {
        public ItemObtainerView ItemObtainerView;

        public bool IsObtainerSystemSubscribed;

        public Stack<ItemView> Items;
    }
}