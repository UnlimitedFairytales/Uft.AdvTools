#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdStopBgm : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Sound;

        protected float FadeOutSeconds { get; set; }

        public CmdStopBgm(float? fadeOutSeconds)
        {
            this.FadeOutSeconds = fadeOutSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.SoundManager.StopBgm(this.FadeOutSeconds);
        }
    }
}
