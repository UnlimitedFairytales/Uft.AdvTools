using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Uft.AdvTools.Commands;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;

namespace Uft.AdvTools
{
    public class ScenarioExecutor
    {
        protected enum CommandReadMode
        {
            Normal,
            Selection,
            WaitingForSelected,
        }

        public bool IsAutoNext { get; set; }

        public bool IsWaiting { get; set; }
        public bool IsWaitingForInput { get; set; }

        protected IReadOnlyList<ICommand> CommandList { get; set; }
        protected int SeekPoint { get; set; }
        protected CommandReadMode ReadMode { get; set; } = CommandReadMode.Normal;
        protected List<CmdSelection> SelectionList { get; private set; } = new();

        public ScenarioExecutor(IReadOnlyList<ICommand> commandList)
        {
            this.CommandList = commandList;
        }

        public virtual void UpdateFrame(AdvRoot advRoot)
        {
            if (this.IsWaitingForInput && this.IsAutoNext && advRoot.AutoNext.IsReady)
            {
                advRoot.Next();
            }

            while (this.SeekPoint < this.CommandList.Count && !this.IsWaiting && !this.IsWaitingForInput && this.ReadMode != CommandReadMode.WaitingForSelected)
            {
                // NOTE: 宴仕様に準拠させる。改ページ直後に演出系コマンドが来た場合、MessageAreaを非表示にする
                if (advRoot.EmulatesUtageEffectCommand)
                {
                    if (this.CommandList[this.SeekPoint].CommandCategory == CommandCategory.Effect && advRoot.MessageArea.LastPageCtrl == CmdText.PageCtrlType.InputBrPage)
                    {
                        advRoot.HideUI();
                    }
                }
                advRoot.MessageArea.FixLastPageCtrl();

                // Selection制御
                if (this.ReadMode == CommandReadMode.Selection && this.CommandList[this.SeekPoint] is not CmdSelection)
                {
                    this.ReadMode = CommandReadMode.WaitingForSelected;
                    UniTask.Void(async () =>
                    {
                        try
                        {
                            var visibleList = this.SelectionList
                                .Where(s => s.IsVisible(advRoot))
                                .ToList();
                            var result = await advRoot.SelectionList.ShowAsync(visibleList);
                            if (result.Value == null || result.Status != View.OperationResultStatus.Accepted) return;

                            var cmdSelection = result.Value;
                            if (!string.IsNullOrWhiteSpace(cmdSelection.OnSelectExpression))
                            {
                                Param.AssignIntParamFromExpression(cmdSelection.OnSelectExpression, advRoot);
                            }
                            this.JumpTo(cmdSelection.ScenarioLabel);
                        }
                        catch (Exception ex)
                        {
                            DevLog.LogError($"[{nameof(ScenarioExecutor)}] Selection handling error\n{ex.Message}");
                            throw;
                        }
                        finally
                        {
                            this.ReadMode = CommandReadMode.Normal;
                        }
                    });
                    break;
                }

                // 読み取り・実行
                this.CommandList[this.SeekPoint++].Run(this, advRoot);
            }
        }

        public virtual void AddCmdSelection(CmdSelection cmdSelection)
        {
            if (this.ReadMode != CommandReadMode.Selection)
            {
                this.SelectionList.Clear();
                this.ReadMode = CommandReadMode.Selection;
            }
            this.SelectionList.Add(cmdSelection);
        }

        public virtual void JumpTo(string scenarioLabel)
        {
            // HACK: scenarioLabel は、Dictionaryで検索かけられるようにするべき
            for (int i = 0; i < this.CommandList.Count; i++)
            {
                var cmd = this.CommandList[i];
                if (cmd.CommandCategory != CommandCategory.Label) continue;

                var casted = (CmdLabel)cmd;
                if (casted.ScenarioLabel == scenarioLabel)
                {
                    this.SeekPoint = i;
                    return;
                }
            }
            var errorText = $"[{nameof(ScenarioExecutor)}] Jump dest is not found! : scenarioLabel={scenarioLabel}";
            DevLog.LogError(errorText);
            throw new Exception(errorText);
        }
    }
}
