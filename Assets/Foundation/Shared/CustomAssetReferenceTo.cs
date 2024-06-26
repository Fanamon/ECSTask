using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Foundation.Shared
{
    [Serializable]
    public class CustomAssetReferenceTo<TComponent> : AssetReferenceGameObject
    {
        public CustomAssetReferenceTo(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(Object obj)
        {
            return obj is TComponent || (obj is GameObject go && go.TryGetComponent(typeof(TComponent), out _));
        }

        public override bool ValidateAsset(string mainAssetPath)
        {
#if UNITY_EDITOR
            if (typeof(TComponent).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(mainAssetPath)))
                return true;

            var obj = AssetDatabase.LoadAssetAtPath(mainAssetPath, typeof(TComponent));
            return ValidateAsset(obj);
#else
            return false;
#endif
        }
    }
}