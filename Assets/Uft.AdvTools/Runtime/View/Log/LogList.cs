#nullable enable

using System.Collections.Generic;
using Uft.UnityUtils.Common;
using Uft.VirtualizedList;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class LogList : MonoBehaviour, IVList<LogListItem, LogData>
    {
        // Static members

        protected static readonly OperationResult<int> CANCEL_RESULT = new(0, OperationResultStatus.Canceled);

        // Instance members

        // Parameters

        public RectTransform RectTransform => (RectTransform)this.transform;
        [SerializeField] VerticalLayoutGroup? _verticalLayoutGroup; public VerticalLayoutGroup VerticalLayoutGroup => ThrowIf.Unassigned(this._verticalLayoutGroup);
        [SerializeField] int _initialPoolLength = 20; public int InitialPoolLength => this._initialPoolLength;
        [SerializeField] LogListItem? _itemPrototype; public LogListItem ItemPrototype => ThrowIf.Unassigned(this._itemPrototype);

        // Status

        readonly List<LogData> _dataList = new(); public List<LogData> DataList => this._dataList;
        readonly Queue<LogListItem> _pool = new(); public Queue<LogListItem> Pool => this._pool;
        readonly List<LogListItem> _activeList = new(); public List<LogListItem> ActiveList => this._activeList;
        int _activeStartDataIndex = 0;
        public int ActiveStartDataIndex
        {
            get { return this._activeStartDataIndex; }
            set { this._activeStartDataIndex = value; }
        }

        // Unity events

        void Reset()
        {
            if (this._verticalLayoutGroup == null) this._verticalLayoutGroup = this.GetComponent<VerticalLayoutGroup>();
        }

        void Awake() => this.AwakeLogic();

        // Methods

        public void AddItemData(LogData data)
        {
            this.DataList.Add(data);
            this.RefreshVirtualContentSize();
        }

        public void RefreshVirtualContentSize() => ((IVList<LogListItem, LogData>)this).RefreshVirtualContentHeight();

        void AwakeLogic() => ((IVList<LogListItem, LogData>)this).AwakeLogic();
    }
}
