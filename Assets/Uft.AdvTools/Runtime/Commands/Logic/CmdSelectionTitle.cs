#nullable enable

namespace Uft.AdvTools.Commands
{
    public class CmdSelectionTitle : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Logic;

        // NOTE: データとして転用するのでpublicアクセス可能にする
        public string? Title { get; protected set; }

        public CmdSelectionTitle(string? title)
        {
            this.Title = title;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.SetCmdSelectionTitle(this);
        }
    }
}
