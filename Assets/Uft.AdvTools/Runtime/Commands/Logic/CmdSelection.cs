#nullable enable

using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;

namespace Uft.AdvTools.Commands
{
    public class CmdSelection : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Logic;

        // NOTE: データとして転用するのでpublicアクセス可能にする
        public string ScenarioLabel { get; protected set; }
        public string? DisplayCondition { get; protected set; }
        public string? OnSelectExpression { get; protected set; }
        public float? OffsetX { get; protected set; } // NOTE: 未使用
        public float? OffsetY { get; protected set; } // NOTE: 未使用
        public string Text { get; protected set; }

        public CmdSelection(string scenarioLabel, string? displayCondition, string? onSelectExpression, float? offsetX, float? offsetY, string text)
        {
            this.ScenarioLabel = scenarioLabel;
            this.DisplayCondition = displayCondition;
            this.OnSelectExpression = onSelectExpression;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.Text = text;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.AddCmdSelection(this);
        }

        // NOTE: データとして転用するので、自己説明的な処理を追加。IsVisible

        public virtual bool IsVisible(AdvRoot advRoot)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.DisplayCondition)) return true;
                return Param.EvaluateBoolean(this.DisplayCondition, advRoot);
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(CmdSelection)}] DisplayCondition={this.DisplayCondition}\n{ex.Message}");
                throw;
            }
        }
    }
}
