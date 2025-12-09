#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using Uft.AdvTools.Commands;
using Uft.UnityUtils;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class SelectionList : MonoBehaviour
    {
        protected static readonly OperationResult<CmdSelection?> CANCEL_RESULT = new(null, OperationResultStatus.Canceled);

        // Parameters

        [SerializeField] protected SelectionTitle? _selectionTitle;
        [SerializeField] protected SelectionItem? _selectionItemPrototype;
        [SerializeField] protected int _maxListLength = 16;
        [SerializeField] protected int _itemSpacing = 70;

        // Status

        protected List<SelectionItem> _selectionItemList = new();
        protected SelectionItem? _selected;

        // Unity events

        protected void Awake()
        {
            if (this._selectionTitle != null)
            {
                this._selectionTitle.gameObject.SetActive(false);
            }
            for (int i = 0; i < this._maxListLength; i++)
            {
                var awaken = ComponentUtil.Instantiate(this._selectionItemPrototype, this.transform, false, true);
                awaken!.gameObject.SetActive(false);
                awaken.Button.onClick.AddListener(() => this.OnSelectionItemClicked(awaken));
                this._selectionItemList.Add(awaken);
            }
        }

        // Methods

        public virtual async UniTask<OperationResult<CmdSelection?>> ShowAsync(string? title, List<CmdSelection> data)
        {
            // 引数チェック
            if (data.Count == 0) return CANCEL_RESULT;

            // Awake保証
            this.gameObject.SetActive(true);

            // クリア
            this._selected = null;
            for (int i = 0; i < this._selectionItemList.Count; i++)
            {
                this._selectionItemList[i].Clear();
                this._selectionItemList[i].CanvasGroup.DOComplete();
                this._selectionItemList[i].gameObject.SetActive(false);
            }

            // 設定・表示
            var ease = Ease.OutQuad;
            var tasks = new List<UniTask>();
            int length = Mathf.Min(this._selectionItemList.Count, data.Count);
            var center = (length - 1) / 2.0f;
            for (int i = 0; i < length; i++)
            {
                var pos = new Vector2(0, this._itemSpacing * (center - i));
                this._selectionItemList[i].SetData(data[i], pos);
                this._selectionItemList[i].gameObject.SetActive(true);

                var canvasGroup = this._selectionItemList[i].CanvasGroup;
                canvasGroup.alpha = 0;
                tasks.Add(canvasGroup.DOFade(1, 0.2f).SetEase(ease).AwaitForComplete());
            }
            if (this._selectionTitle != null && title != null)
            {
                this._selectionTitle.SetData(title, new Vector2(0, this._itemSpacing * (center + 1)));
                this._selectionTitle.gameObject.SetActive(true);

                var canvasGroup = this._selectionTitle.CanvasGroup;
                canvasGroup.alpha = 0;
                tasks.Add(canvasGroup.DOFade(1, 0.2f).SetEase(ease).AwaitForComplete());
            }
            await UniTask.WhenAll(tasks);
            tasks.Clear();

            // 選択待ち
            await UniTask.WaitUntil(() => this == null || !this.gameObject.activeSelf || this._selected != null);
            if (this == null)
            {
                return CANCEL_RESULT;
            }
            if (!this.gameObject.activeSelf && this._selected == null)
            {
                return CANCEL_RESULT;
            }

            // 非表示
            for (int i = 0; i < length; i++)
            {
                this._selectionItemList[i].CanvasGroup.DOComplete();
                tasks.Add(this._selectionItemList[i].CanvasGroup.DOFade(0, 0.2f).SetEase(ease).AwaitForComplete());
            }
            if (this._selectionTitle != null)
            {
                tasks.Add(this._selectionTitle.CanvasGroup.DOFade(0, 0.2f).SetEase(ease).AwaitForComplete());
            }
            await UniTask.WhenAll(tasks);
            if (this._selectionTitle != null)
            {
                this._selectionTitle.gameObject.SetActive(false);
            }
            for (int i = 0; i < length; i++)
            {
                this._selectionItemList[i].gameObject.SetActive(false);
            }

            // 返却
            if (this._selected == null) return CANCEL_RESULT;
            return new OperationResult<CmdSelection?>(this._selected.CmdSelection, OperationResultStatus.Accepted);
        }

        public virtual async UniTask CloseAsync()
        {
            // HACK: フェード対応するべき
            await UniTask.Delay(0);
            this.gameObject.SetActive(false);
        }

        protected virtual void OnSelectionItemClicked(SelectionItem sender) => this._selected = sender;
    }
}
