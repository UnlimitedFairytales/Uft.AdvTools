#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdSpriteOff : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Object;

        protected string Label { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdSpriteOff(string textureLabel, float? fadeSeconds)
        {
            this.Label = textureLabel;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var textureRow = advRoot.SpriteDictionary[this.Label];
            advRoot.SpriteManager.SetSpriteOff(textureRow.Sprite, this.FadeSeconds);
        }
    }
}
