#nullable enable

using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Entities
{
    public class TextureRow
    {
        // operator, Equals, GetHashCode, ToString

        public override string ToString() => $"{this.Label},{this.Type},{this.OffsetX},{this.OffsetY},{this.Pivot},{this.Scale},{this.Sprite}";

        // Parameters

        public string Label { get; protected set; } // key
        public string Type { get; protected set; }
        public float OffsetX { get; protected set; }
        public float OffsetY { get; protected set; }
        public AnchorPreset Pivot { get; protected set; }
        public float Scale { get; protected set; }
        public Sprite? Sprite { get; protected set; }

        public GameObject? Prefab { get; protected set; }
        GameObject? _instantiated = null;
        public GameObject? Instantiated
        {
            get
            {
                this._instantiated = CacheUtil.GetCreatedObject(this._instantiated, this.Prefab);
                return this._instantiated;
            }
        }
        bool _hasTriedToGetAnimator;
        Animator? _animator = null;
        public Animator? Animator
        {
            get
            {
                if (this.Instantiated == null) return null;
                if (this._hasTriedToGetAnimator) return this._animator;
                this._hasTriedToGetAnimator = true;
                this._animator = CacheUtil.GetCachedChildComponent(this._animator, this.Instantiated.transform, true);
                return this._animator;
            }
        }

        // Status

        /// <summary>初期値、非表示後（SpriteOff、BgOffなど）はParameter値になる想定。</summary>
        public int LastImageIndex { get; set; }

        // Methods

        public TextureRow(string label, string type, float? offsetX, float? offsetY, AnchorPreset? pivot, float? scale, Sprite? sprite, GameObject? prefab)
        {
            this.Label = label;
            this.Type = type;
            this.OffsetX = offsetX ?? 0;
            this.OffsetY = offsetY ?? 0;
            this.Pivot = pivot ?? AnchorPreset.MiddleCenter;
            this.Scale = scale ?? 1.0f;
            this.Sprite = sprite;
            this.Prefab = prefab;

            this.ResetLastStatus();
        }

        public void ResetLastStatus()
        {
            this.LastImageIndex = 0;
        }
    }
}
