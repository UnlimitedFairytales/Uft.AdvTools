#nullable enable

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Uft.AdvTools.Commands;
using Uft.AdvTools.View;
using Uft.UnityUtils;
using UnityEngine;

namespace Uft.AdvTools
{
    public class SelectionList
    {
        protected static readonly OperationResult<CmdSelection?> CANCEL_RESULT = new(OperationResultStatus.Canceled, null);

        readonly SelectionListView _view;
        readonly IInputProxy _inputProxy;

        int _focusIndex;
        List<CmdSelection>? _data;

        public SelectionList(SelectionListView view, IInputProxy inputProxy)
        {
            this._view = view;
            this._inputProxy = inputProxy;
        }

        public virtual async UniTask<OperationResult<CmdSelection?>> ShowDialogAsync(string? title, List<CmdSelection> data)
        {
            // 引数チェック
            if (data.Count == 0) return CANCEL_RESULT;

            this._focusIndex = 0;
            this._data = data;
            CmdSelection? selected = null;

            this._view.OnSelectionItemClicked = (index) =>
            {
                this._focusIndex = Mathf.Clamp(index, 0, this._data.Count - 1);
                this._view.SetFocus(this._focusIndex);
                if (this._data != null)
                {
                    selected = this._data[this._focusIndex];
                }
            };
            await this._view.ShowAsync(title, data);

            var length = data.Count;
            while (selected == null && this._view != null && this._view.gameObject.activeSelf)
            {
                if (this._inputProxy.Up(InputState.Triggered))
                {
                    this._focusIndex = Mathf.Max(0, this._focusIndex - 1);
                    this._view.SetFocus(this._focusIndex);
                }
                if (this._inputProxy.Down(InputState.Triggered))
                {
                    this._focusIndex = Mathf.Min(this._focusIndex + 1, length - 1);
                    this._view.SetFocus(this._focusIndex);
                }
                if (this._inputProxy.Submit(InputState.Triggered))
                {
                    selected = this._data[this._focusIndex];
                }
                await UniTask.NextFrame();
            }

            // 後処理
            if (this._view != null)
            {
                this._view.OnSelectionItemClicked = null;
                await this._view.HideAsync();
            }

            // 返却
            if (selected == null) return CANCEL_RESULT;
            return new OperationResult<CmdSelection?>(OperationResultStatus.Accepted, selected);
        }
    }
}
