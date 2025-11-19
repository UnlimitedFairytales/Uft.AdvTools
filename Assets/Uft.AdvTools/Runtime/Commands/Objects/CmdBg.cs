#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdBg : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected string Label { get; set; }
        protected float? OffsetX { get; set; }
        protected float? OffsetY { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdBg(string textureLabel, float? offsetX, float? offsetY, float? fadeSeconds)
        {
            this.Label = textureLabel;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var textureRow = advRoot.BgDictionary[this.Label];
            var x = this.OffsetX ?? textureRow.LastOffsetX;
            var y = this.OffsetY ?? textureRow.LastOffsetY;
            var pivot = textureRow.Pivot;
            var scale = textureRow.Scale;

            advRoot.Bg.ChangeBg(textureRow.Sprite, x, y, pivot, scale, this.FadeSeconds);

            // NOTE: Bgには LastImageIndex の概念がないため、初期化不要
            textureRow.LastOffsetX = x;
            textureRow.LastOffsetY = y;
        }
    }
}
