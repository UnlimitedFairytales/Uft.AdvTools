#nullable enable

using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;

namespace Uft.AdvTools.Commands
{
    public class CmdJump : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Logic;

        protected string ScenarioLabel { get; set; }
        protected string Expression { get; set; }

        public CmdJump(string scenarioLabel, string expression)
        {
            this.ScenarioLabel = scenarioLabel;
            this.Expression = expression;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            try
            {
                if (Param.EvaluateBoolean(this.Expression, advRoot))
                {
                    scenarioExecutor.JumpTo(this.ScenarioLabel);
                }
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(CmdJump)}] Expression={this.Expression}\n{ex.Message}");
                throw;
            }
        }
    }
}
