#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdHideMessageWindow : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.UI;

        public CmdHideMessageWindow() { }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            advRoot.HideUI();
        }
    }
}
