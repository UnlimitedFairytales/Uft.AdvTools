#nullable enable

using Uft.AdvTools.View;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdSprite : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected string Label { get; set; }
        protected int? ImageIndex { get; set; }
        protected float? PosX { get; set; }
        protected float? PosY { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdSprite(string textureLabel, int? imageIndex, float? posX, float? posY, float? fadeSeconds)
        {
            this.Label = textureLabel;
            this.ImageIndex = imageIndex is int idx ?
                Mathf.Clamp(idx, 0, SpriteManager.IMG_COUNT - 1) :
                null;
            this.PosX = posX;
            this.PosY = posY;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var textureRow = advRoot.SpriteDictionary[this.Label];
            var imageIndex = this.ImageIndex ?? textureRow.LastImageIndex;
            advRoot.SpriteManager.SetSprite(textureRow, imageIndex, this.PosX, this.PosY, this.FadeSeconds);

            textureRow.LastImageIndex = imageIndex;
        }
    }
}
