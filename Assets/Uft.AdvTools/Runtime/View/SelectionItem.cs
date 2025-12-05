#nullable enable

using TMPro;
using Uft.AdvTools.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class SelectionItem : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected CanvasGroup? _canvasGroup; public CanvasGroup CanvasGroup => this._canvasGroup!;
        [SerializeField] protected Button? _button; public Button Button => this._button!;
        [SerializeField] protected TMP_Text? _txtText;

        // Status

        public CmdSelection? CmdSelection { get; protected set; }

        // Methods

        public void SetData(CmdSelection data, Vector2 anchoredPosition)
        {
            this.CmdSelection = data;
            if (this._txtText != null) this._txtText.text = data.Text;
            ((RectTransform)this.transform).anchoredPosition = anchoredPosition;
        }

        public virtual void Clear()
        {
            ((RectTransform)this.transform).anchoredPosition = Vector2.zero;
            if (this._txtText != null) this._txtText.text = string.Empty;
            this.CmdSelection = null;
        }
    }
}
