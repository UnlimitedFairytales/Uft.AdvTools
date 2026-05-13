#nullable enable

using System;
using UnityEngine;

namespace Uft.AdvTools
{
    public class AssetLoadProxy
    {
        public enum LoadType
        {
            Resources,
            ExternalLoader,
        }

        public LoadType loadType = LoadType.Resources;

        readonly Func<string, Type, UnityEngine.Object> _externalLoader;

        public AssetLoadProxy(Func<string, Type, UnityEngine.Object> externalLoader)
        {
            this._externalLoader = externalLoader;
        }

        public T Load<T>(string path) where T : UnityEngine.Object
        {

            if (this.loadType == LoadType.Resources)
            {
                return Resources.Load<T>(path);
            }
            else
            {
                return (T)this._externalLoader(path, typeof(T));
            }
        }
    }
}
