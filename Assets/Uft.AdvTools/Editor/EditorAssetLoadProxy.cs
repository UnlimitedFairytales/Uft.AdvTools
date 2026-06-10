#nullable enable

using System;
using System.IO;
using UnityEditor;

namespace Uft.AdvTools.Editor
{
    /// <summary>Editor専用。AssetDatabaseを使ってアセットをロードするAssetLoadProxyを生成する</summary>
    static class EditorAssetLoadProxy
    {
        /// <summary>拡張子なしパスを受け取り、同フォルダ内で名前一致するアセットをAssetDatabaseから返すProxyを生成する</summary>
        public static AssetLoadProxy Create() => new AssetLoadProxy(Load);

        static UnityEngine.Object Load(string path, Type type)
        {
            var dir = Path.GetDirectoryName(path)?.Replace('\\', '/') ?? "Assets";
            var name = Path.GetFileNameWithoutExtension(path);
            var guids = AssetDatabase.FindAssets($"{name} t:{type.Name}", new[] { dir });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.Equals(Path.GetFileNameWithoutExtension(assetPath), name, StringComparison.OrdinalIgnoreCase))
                {
                    var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
                    if (asset != null) return asset;
                }
            }
            return null!;
        }
    }
}
