#nullable enable

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;
using Uft.VirtualizedList;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class LogController : MonoBehaviour, IVController<LogList, LogListItem, LogData>
    {
        // Parameters

        [SerializeField] ScrollRect? _scrollRect; public ScrollRect ScrollRect => ThrowIf.Unassigned(this._scrollRect);
        [SerializeField] LogList? _list; public LogList Content => ThrowIf.Unassigned(this._list);
        [SerializeField] float _margin = 50f; public float Margin => this._margin;

        // Status

        public Transform Transform => this.transform;

        // Unity events

        protected virtual void Reset()
        {
            if (this._scrollRect == null) this._scrollRect = this.GetComponent<ScrollRect>();
            if (this._list == null) this._list = this.GetComponentInChildren<LogList>();
        }

        protected virtual void Update()
        {
            if (this._scrollRect == null) throw new UnassignedReferenceException(nameof(this._scrollRect));
            if (this._list == null) throw new UnassignedReferenceException(nameof(this._list));

            this.Tick();
        }

        // Methods

        public virtual int GetIndexAtPosition(float posY)
        {
            if (this._scrollRect == null) throw new UnassignedReferenceException(nameof(this._scrollRect));
            if (this._list == null) throw new UnassignedReferenceException(nameof(this._list));

            var layoutGroup = this._list.VerticalLayoutGroup;
            if (layoutGroup == null) return 0;

            float y = 0;
            y += layoutGroup.padding.top;
            for (int i = 0; i < this._list.DataList.Count; i++)
            {
                var itemH = this._list.ItemPrototype.CalcHeight(this._list.DataList[i]);
                if (posY < y + itemH) return i;
                y += itemH + layoutGroup.spacing;
            }
            return this._list.DataList.Count - 1;
        }

        void Tick() => ((IVController<LogList, LogListItem, LogData>)this).Tick();

        // Show, Close

        static readonly OperationResult<int> CANCEL_RESULT = new(OperationResultStatus.Canceled, 0);

        public virtual async UniTask<OperationResult<int>> ShowAsync(IReadOnlyList<LogData> dataList)
        {
            if (this._scrollRect == null) throw new UnassignedReferenceException(nameof(this._scrollRect));
            if (this._list == null) throw new UnassignedReferenceException(nameof(this._list));

            // Awake保証
            this.gameObject.SetActive(true);

            this._list.DataList.Clear();
            for (int i = 0; i < dataList.Count; i++)
            {
                this._list.DataList.Add(dataList[i]);
            }
            this._list.RefreshVirtualContentSize();

            // 表示終了待ち
            await UniTask.WaitUntil(() => this == null || !this.gameObject.activeSelf);
            if (this == null) return CANCEL_RESULT;

            // 返却
            return CANCEL_RESULT;
        }

        public virtual async UniTask CloseAsync()
        {
            // HACK: フェード対応するべき
            await UniTask.Delay(0);
            this.gameObject.SetActive(false);
        }
    }
}
