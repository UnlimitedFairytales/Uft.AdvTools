#nullable enable

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

        public Action? OnSelectionShowing { get; set; }
        public Action? OnSelectionHidden { get; set; }

        public bool IsWaiting { get; set; }
        public bool IsWaitingForInput { get; set; }
        public bool IsAnyWaiting => this.IsWaiting || this.IsWaitingForInput;

        public bool IsEndScenario => (this.CommandList.Count <= this.SeekPoint) && !this.IsWaiting && !this.IsWaitingForInput;
        public bool IsPauseScenario { get; protected set; }
        public bool IsEndOrPauseScenario => this.IsEndScenario || this.IsPauseScenario;

        protected IReadOnlyList<ICommand> CommandList { get; set; }
        protected int SeekPoint { get; set; }
        protected CommandReadMode ReadMode { get; set; } = CommandReadMode.Normal;
        protected CmdSelectionTitle? SelectionTitle { get; private set; }
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

            while (!this.IsAnyWaiting && !this.IsEndOrPauseScenario && this.ReadMode != CommandReadMode.WaitingForSelected)
            {
                // NOTE: 宴仕様に準拠させる。改ページ直後に演出系コマンドが来た場合、MessageWindowを非表示にする
                if (advRoot.EmulatesUtageEffectCommand)
                {
                    if (this.CommandList[this.SeekPoint].CommandCategory == CommandCategory.Effect && advRoot.MessageWindowManager.CurrentMessageWindow.LastPageCtrl == CmdText.PageCtrlType.InputBrPage)
                    {
                        advRoot.HideUI(0);
                    }
                }
                advRoot.MessageWindowManager.CurrentMessageWindow.FixLastPageCtrl();

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
                            var logic = new SelectionList(advRoot.SelectionListView, advRoot.InputProxy);
                            this.OnSelectionShowing?.Invoke();
                            var result = await logic.ShowDialogAsync(advRoot.destroyCancellationToken, this.SelectionTitle?.Title, visibleList);
                            this.OnSelectionHidden?.Invoke();
                            if (result.value == null || result.status != OperationResultStatus.Accepted) return;

                            var cmdSelection = result.value;

                            var lastPageCtrl = CmdText.PageCtrlType.InputBrPage;
                            advRoot.MessageWindowManager.CurrentMessageWindow.ForceSetLastPageCtrl(lastPageCtrl);
                            advRoot.LogManager.Add(lastPageCtrl, null, "", "[" + cmdSelection.Text + "]");
                            if (!string.IsNullOrWhiteSpace(cmdSelection.OnSelectExpression))
                            {
                                Param.AssignIntParamFromExpression(cmdSelection.OnSelectExpression, advRoot);
                            }
                            this.JumpTo(cmdSelection.ScenarioLabel);
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException)
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

        public virtual void SetCmdSelectionTitle(CmdSelectionTitle CmdSelectionTitle)
        {
            if (this.ReadMode != CommandReadMode.Selection)
            {
                this.SelectionTitle = null;
                this.SelectionList.Clear();
                this.ReadMode = CommandReadMode.Selection;
            }
            this.SelectionTitle = CmdSelectionTitle;
        }

        public virtual void AddCmdSelection(CmdSelection cmdSelection)
        {
            if (this.ReadMode != CommandReadMode.Selection)
            {
                this.SelectionTitle = null;
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

        public virtual void PauseScenario() => this.IsPauseScenario = true;

        public virtual void ResumeScenario() => this.IsPauseScenario = false;
    }
}
