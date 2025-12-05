#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdShowMessageWindow : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.UI;

        public CmdShowMessageWindow() { }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.ShowUI();
        }
    }
}
