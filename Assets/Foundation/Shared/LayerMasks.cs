using UnityEngine;

namespace Foundation.Shared
{
    internal static class LayerMasks
    {
        private const string Player = nameof(Player);
        private const string Item = nameof(Item);

        public static LayerMask AntispawnLayer => LayerMask.GetMask(Player) | 
            LayerMask.GetMask(Item);
    }
}