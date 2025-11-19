#nullable enable

using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdSprite : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected string Label { get; set; }
        protected int? ImageIndex { get; set; }
        protected float? OffsetX { get; set; }
        protected float? OffsetY { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdSprite(string textureLabel, int? imageIndex, float? offsetX, float? offsetY, float? fadeSeconds)
        {
            this.Label = textureLabel;
            this.ImageIndex = imageIndex is int idx ?
                Mathf.Clamp(idx, 0, SpriteManager.IMG_COUNT - 1) :
                null;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var textureRow = advRoot.SpriteDictionary[this.Label];

            var sprite = textureRow.Sprite;
            var imageIndex = this.ImageIndex ?? textureRow.LastImageIndex;
            var x = this.OffsetX ?? textureRow.LastOffsetX;
            var y = this.OffsetY ?? textureRow.LastOffsetY;
            var pivot = textureRow.Pivot;
            var scale = textureRow.Scale;

            advRoot.SpriteManager.SetSprite(sprite, imageIndex, x, y, pivot, scale, this.FadeSeconds);

            textureRow.LastImageIndex = imageIndex;
            textureRow.LastOffsetX = x;
            textureRow.LastOffsetY = y;
        }
    }
}
