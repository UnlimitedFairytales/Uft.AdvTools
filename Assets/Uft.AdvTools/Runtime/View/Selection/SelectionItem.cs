#nullable enable

using TMPro;
using Uft.AdvTools.Commands;
using Uft.UnityUtils.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    [RequireComponent(typeof(RectTransform))]
    public class SelectionItem : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected CanvasGroup? _canvasGroup; public CanvasGroup CanvasGroup => ThrowIf.Unassigned(this._canvasGroup);
        [SerializeField] protected Button? _button; public Button Button => ThrowIf.Unassigned(this._button);
        [SerializeField] protected TMP_Text? _txtText;

        // Status

        public CmdSelection? CmdSelection { get; protected set; }

        // Methods

        public virtual void SetData(CmdSelection data, Vector2 anchoredPosition)
        {
            if (this._canvasGroup == null) throw new UnassignedReferenceException(nameof(this._canvasGroup));
            if (this._button == null) throw new UnassignedReferenceException(nameof(this._button));
            if (this._txtText == null) throw new UnassignedReferenceException(nameof(this._txtText));

            this.CmdSelection = data;
            this._txtText.text = data.Text;
            ((RectTransform)this.transform).anchoredPosition = anchoredPosition;
        }

        public virtual void Clear()
        {
            if (this._canvasGroup == null) throw new UnassignedReferenceException(nameof(this._canvasGroup));
            if (this._button == null) throw new UnassignedReferenceException(nameof(this._button));
            if (this._txtText == null) throw new UnassignedReferenceException(nameof(this._txtText));

            ((RectTransform)this.transform).anchoredPosition = Vector2.zero;
            this._txtText.text = string.Empty;
            this.CmdSelection = null;
        }

        public virtual void SetFocus()
        {
            if (this._button == null) throw new UnassignedReferenceException(nameof(this._button));
            this._button.Select();
        }
    }
}
