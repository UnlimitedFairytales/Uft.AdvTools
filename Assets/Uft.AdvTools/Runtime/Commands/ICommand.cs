namespace Uft.AdvTools.Commands
{
    public interface ICommand
    {
        public CommandCategory CommandCategory { get; }

        public void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot);
    }
}
