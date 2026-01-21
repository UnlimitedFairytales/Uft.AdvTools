#nullable enable

using TMPro;
using Uft.UnityUtils.Common;
using Uft.VirtualizedList;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class LogListItem : MonoBehaviour, IVListItem<LogData>
    {
        // Parameters

        public RectTransform RectTransform => (RectTransform)this.transform;
        [SerializeField] LayoutElement? _layoutElement; public LayoutElement LayoutElement => ThrowIf.Unassigned(this._layoutElement);
        [SerializeField] RectTransform? _childVariableSizeSource; public RectTransform ChildVariableSizeSource => ThrowIf.Unassigned(this._childVariableSizeSource);
        [SerializeField] float _fixedOtherWidth; public float FixedOtherWidth => this._fixedOtherWidth;
        [SerializeField] float _fixedOtherHeight; public float FixedOtherHeight => this._fixedOtherHeight;
        [SerializeField] float _fallbackChildVariableWidth; public float FallbackChildVariableWidth => this._fallbackChildVariableWidth;
        [SerializeField] float _fallbackChildVariableHeight; public float FallbackChildVariableHeight => this._fallbackChildVariableHeight;

        // 仮想計算用
        [SerializeField] float _virtualLineHeight = 30f; protected float VirtualLineHeight => this._virtualLineHeight;
        [SerializeField] float _virtualPadding = 20f; protected float VirtualPadding => this._virtualPadding;

        // 実際のParameter
        [SerializeField] TMP_Text? _txtName; protected TMP_Text? TxtName => this._txtName;
        [SerializeField] TMP_Text? _txtText; protected TMP_Text? TxtText => this._txtText;

        // Status

        public LogData? LogItem { get; protected set; }

        // Methods

        public virtual void SetData(LogData data)
        {
            if (this._txtText == null) throw new UnassignedReferenceException(nameof(this._txtText));

            if (this._txtName != null) this._txtName.text = data.Name; // NOTE: _txtNameは未設定を許容する
            this._txtText.text = data.Text;
            this.RefreshLayoutElement();
        }

        public virtual float CalcHeight(LogData data)
        {
            string text = data.Text;
            int lineCount = CountLines(text);
            float itemHeight = this._virtualPadding + this._virtualLineHeight * lineCount;
            return itemHeight;

            // 折り返しない前提で行数を計算
            static int CountLines(string text)
            {
                if (string.IsNullOrEmpty(text)) return 1;

                int count = 1;
                var prevIsTmpNewLineFirstChar = false;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n') count++;
                    if (prevIsTmpNewLineFirstChar && text[i] == 'n') count++;
                    prevIsTmpNewLineFirstChar = (text[i] == '\\');
                }
                return count;
            }
        }

        void RefreshLayoutElement() => ((IVListItem<LogData>)this).RefreshLayoutElement();
    }
}
