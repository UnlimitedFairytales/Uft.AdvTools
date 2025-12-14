#nullable enable

using System;
using System.Collections.Generic;

namespace Uft.AdvTools.Entities
{
    public readonly struct FileType : IEquatable<FileType>
    {
        // operator, Equals, GetHashCode, ToString

        public static bool operator ==(FileType a, FileType b) => a.value == b.value;
        public static bool operator !=(FileType a, FileType b) => a.value != b.value;
        public override bool Equals(object? obj) => obj is FileType other && this.Equals(other);
        public readonly bool Equals(FileType other) => this.value == other.value;
        public override int GetHashCode() => this.value?.GetHashCode() ?? 0;
        public override string ToString() => this.value ?? "";

        // static

        const string NONE = "";
        const string _2D_PREFAB = "2DPrefab";
        const string _3D_PREFAB = "3DPrefab";

        public static FileType None { get; } = new(NONE);
        public static FileType Prefab2D { get; } = new(_2D_PREFAB);
        public static FileType Prefab3D { get; } = new(_3D_PREFAB);

        public static IReadOnlyList<FileType> Known { get; } = new[] { None, Prefab2D, Prefab3D };

        public static bool TryParse(string? s, out FileType fileType)
        {
            if (string.IsNullOrEmpty(s))
            {
                fileType = None;
                return true;
            }
            // NOTE: 大文字小文字区別する仕様
            fileType = s switch
            {
                _2D_PREFAB => Prefab2D,
                _3D_PREFAB => Prefab3D,
                _ => default,
            };
            return fileType != default;
        }

        // instance

        public readonly string value;

        private FileType(string value) => this.value = value;
    }
}
