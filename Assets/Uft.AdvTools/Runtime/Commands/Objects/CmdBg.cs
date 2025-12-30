#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdBg : ICommand
    {
        // Static members

        public const string SPRITE_NAME_BG = "BG";

        // Instance members

        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected string Label { get; set; }
        protected float? PosX { get; set; }
        protected float? PosY { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdBg(string textureLabel, float? posX, float? posY, float? fadeSeconds)
        {
            this.Label = textureLabel;
            this.PosX = posX;
            this.PosY = posY;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var textureRow = advRoot.BgDictionary[this.Label];
            advRoot.Bg.ChangeBg(textureRow, this.PosX, this.PosY, this.FadeSeconds);
        }
    }
}
