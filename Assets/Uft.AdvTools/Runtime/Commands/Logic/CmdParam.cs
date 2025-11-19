#nullable enable

using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;

namespace Uft.AdvTools.Commands
{
    public class CmdParam : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Logic;

        protected string Expression { get; set; }

        public CmdParam(string expression)
        {
            this.Expression = expression;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            try
            {
                Param.AssignIntParamFromExpression(this.Expression, advRoot);
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(CmdParam)}] Expression={this.Expression}\n{ex.Message}");
                throw;
            }
        }
    }
}
