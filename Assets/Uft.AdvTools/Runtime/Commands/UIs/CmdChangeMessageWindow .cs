#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdChangeMessageWindow : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.UI;

        public string MessageWindowName { get; protected set; }
        /// <summary>独自拡張</summary>
        protected float FadeSeconds { get; set; }

        public CmdChangeMessageWindow(string messageWindowName, float? fadeSeconds)
        {
            this.MessageWindowName = messageWindowName;
            this.FadeSeconds = fadeSeconds ?? 0;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.MessageWindowManager.ChangeMessageWindow(this.MessageWindowName, this.FadeSeconds);
        }
    }
}
