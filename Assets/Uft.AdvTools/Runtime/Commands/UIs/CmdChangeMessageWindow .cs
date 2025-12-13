#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdChangeMessageWindow : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.UI;

        public string MessageWindowName { get; protected set; }

        public CmdChangeMessageWindow(string messageWindowName)
        {
            this.MessageWindowName = messageWindowName;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.MessageWindowManager.ChangeMessageWindow(this.MessageWindowName);
        }
    }
}
