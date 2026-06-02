#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using Uft.AdvTools.Commands;
using Uft.UnityUtils;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class SelectionListView : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected SelectionTitle? _selectionTitle;
        [SerializeField] protected SelectionItem? _selectionItemPrototype;
        [SerializeField] protected int _maxListLength = 16;
        [SerializeField] protected int _itemSpacing = 70;

        // Status

        public Action<int>? OnSelectionItemClicked;

        protected List<SelectionItem> _selectionItemList = new();
        protected int _length;

        // Unity events

        protected virtual void Awake()
        {
            if (this._selectionItemPrototype == null) throw new UnassignedReferenceException(nameof(this._selectionItemPrototype));

            if (this._selectionTitle != null) // NOTE: _selectionTitleは未設定を許容する
            {
                this._selectionTitle.gameObject.SetActive(false);
            }
            for (int i = 0; i < this._maxListLength; i++)
            {
                var index = i; // NOTE: キャプチャ
                var awaken = ComponentUtil.Instantiate(this._selectionItemPrototype, this.transform, false, true);
                awaken!.gameObject.SetActive(false);
                awaken.Button.onClick.AddListener(() => this.OnSelectionItemClicked?.Invoke(index));
                this._selectionItemList.Add(awaken);
            }
        }

        // Methods

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken, string? title, List<CmdSelection> data)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, cancellationToken);
            // Awake保証
            this.gameObject.SetActive(true);

            // クリア
            for (int i = 0; i < this._selectionItemList.Count; i++)
            {
                this._selectionItemList[i].Clear();
                this._selectionItemList[i].CanvasGroup.DOComplete();
                this._selectionItemList[i].gameObject.SetActive(false);
            }

            // 設定・表示
            var ease = Ease.OutQuad;
            var tasks = new List<UniTask>();
            this._length = Mathf.Min(this._selectionItemList.Count, data.Count);
            var center = (this._length - 1) / 2.0f;
            for (int i = 0; i < this._length; i++)
            {
                var pos = new Vector2(0, this._itemSpacing * (center - i));
                this._selectionItemList[i].SetData(data[i], pos);
                this._selectionItemList[i].gameObject.SetActive(true);

                var canvasGroup = this._selectionItemList[i].CanvasGroup;
                canvasGroup.alpha = 0;
                tasks.Add(canvasGroup.DOFade(1, 0.2f).SetEase(ease).AwaitForComplete(cancellationToken: cts.Token));
            }
            if (this._selectionTitle != null && title != null)
            {
                this._selectionTitle.SetData(title, new Vector2(0, this._itemSpacing * (center + 1)));
                this._selectionTitle.gameObject.SetActive(true);

                var canvasGroup = this._selectionTitle.CanvasGroup;
                canvasGroup.alpha = 0;
                tasks.Add(canvasGroup.DOFade(1, 0.2f).SetEase(ease).AwaitForComplete(cancellationToken: cts.Token));
            }
            this.SetFocus(0);
            await UniTask.WhenAll(tasks);
        }

        public virtual async UniTask HideAsync(CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, cancellationToken);
            // 非表示
            var ease = Ease.OutQuad;
            var tasks = new List<UniTask>();
            for (int i = 0; i < this._length; i++)
            {
                this._selectionItemList[i].CanvasGroup.DOComplete();
                tasks.Add(this._selectionItemList[i].CanvasGroup.DOFade(0, 0.2f).SetEase(ease).AwaitForComplete(cancellationToken: cts.Token));
            }
            if (this._selectionTitle != null)
            {
                tasks.Add(this._selectionTitle.CanvasGroup.DOFade(0, 0.2f).SetEase(ease).AwaitForComplete(cancellationToken: cts.Token));
            }
            await UniTask.WhenAll(tasks);
            if (this._selectionTitle != null)
            {
                this._selectionTitle.gameObject.SetActive(false);
            }
            for (int i = 0; i < this._length; i++)
            {
                this._selectionItemList[i].gameObject.SetActive(false);
            }
            this.gameObject.SetActive(false);
        }

        public void SetFocus(int index)
        {
            index = Mathf.Clamp(index, 0, Mathf.Max(0, this._length - 1));
            DevLog.Log($"[{nameof(SelectionListView)}] {nameof(SetFocus)}({index})");
            this._selectionItemList[index].SetFocus();
        }
    }
}
