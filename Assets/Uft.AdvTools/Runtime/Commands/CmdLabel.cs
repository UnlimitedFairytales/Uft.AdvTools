#nullable enable

using System;

namespace Uft.AdvTools.Commands
{
    public class CmdLabel : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Label;

        // NOTE: データとして転用するのでpublicアクセス可能にする
        public string ScenarioLabel { get; protected set; }

        public CmdLabel(string cmd, string sheetName)
        {
            if (cmd.StartsWith("**"))
            {
                // Local label
                this.ScenarioLabel = $"*{sheetName}*{cmd[2..].Trim()}";
                return;
            }
            else if (cmd.StartsWith("*"))
            {
                // Label
                this.ScenarioLabel = cmd.Trim();
            }
            else
            {
                throw new ArgumentException($"{cmd} is not label", nameof(cmd));
            }
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot) { }
    }
}
