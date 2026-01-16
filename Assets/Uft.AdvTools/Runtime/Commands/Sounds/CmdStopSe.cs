#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdStopSe : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Sound;

        protected string? Label { get; set; }
        protected float FadeOutSeconds { get; set; }

        public CmdStopSe(string? soundLabel, float? fadeOutSeconds)
        {
            this.Label = soundLabel;
            this.FadeOutSeconds = fadeOutSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var clip = !string.IsNullOrWhiteSpace(this.Label) ? advRoot.SeDictionary[this.Label] : null;
            advRoot.SoundManager.StopSe(this.FadeOutSeconds, clip);
        }
    }
}
