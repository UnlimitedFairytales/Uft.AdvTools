#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdHideMessageWindow : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.UI;

        /// <summary>独自拡張</summary>
        protected float FadeSeconds { get; set; }

        public CmdHideMessageWindow(float? fadeSeconds)
        {
            this.FadeSeconds = fadeSeconds ?? 0;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.HideUI(this.FadeSeconds);
        }
    }
}
