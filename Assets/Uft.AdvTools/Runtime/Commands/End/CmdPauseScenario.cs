#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdPauseScenario : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.End;

        public CmdPauseScenario()
        {
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.PauseScenario();
        }
    }
}
