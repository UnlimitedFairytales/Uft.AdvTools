#nullable enable

using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Entities
{
    public class TextureRow
    {
        // Parameters

        public string Label { get; protected set; } // key
        public string Type { get; protected set; }
        public float OffsetX { get; protected set; }
        public float OffsetY { get; protected set; }
        public AnchorPreset Pivot { get; protected set; }
        public float Scale { get; protected set; }
        public Sprite Sprite { get; protected set; }

        // Status

        /// <summary>初期値、非表示後（SpriteOff、BgOffなど）はParameter値になる想定。</summary>
        public int LastImageIndex { get; set; }
        /// <summary>初期値、非表示後（SpriteOff、BgOffなど）はParameter値になる想定。</summary>
        public float LastOffsetX { get; set; }
        /// <summary>初期値、非表示後（SpriteOff、BgOffなど）はParameter値になる想定。</summary>
        public float LastOffsetY { get; set; }

        // Methods

        public TextureRow(string label, string type, float? offsetX, float? offsetY, AnchorPreset? pivot, float? scale, Sprite sprite)
        {
            this.Label = label;
            this.Type = type;
            this.OffsetX = offsetX ?? 0;
            this.OffsetY = offsetY ?? 0;
            this.Pivot = pivot ?? AnchorPreset.MiddleCenter;
            this.Scale = scale ?? 1.0f;
            this.Sprite = sprite;

            this.ResetLastStatus();
        }

        public void ResetLastStatus()
        {
            this.LastImageIndex = 0;
            this.LastOffsetX = this.OffsetX;
            this.LastOffsetY = this.OffsetY;
        }

        public override string ToString() => $"{this.Label},{this.Type},{this.OffsetX},{this.OffsetY},{this.Pivot},{this.Scale},{this.Sprite}";
    }
}
