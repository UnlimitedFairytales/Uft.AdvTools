#nullable enable

using System;
using UnityEngine;

namespace Uft.AdvTools
{
    public class AssetLoadProxy
    {
        readonly Func<string, Type, UnityEngine.Object>? _externalLoader;

        public AssetLoadProxy(Func<string, Type, UnityEngine.Object>? externalLoader) => this._externalLoader = externalLoader;

        public T Load<T>(string path) where T : UnityEngine.Object
        {
            return this._externalLoader != null ?
                (T)this._externalLoader(path, typeof(T)) :
                Resources.Load<T>(path);
        }
    }
}
