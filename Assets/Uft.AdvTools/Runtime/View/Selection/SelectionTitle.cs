#nullable enable

using TMPro;
using Uft.UnityUtils.Common;
using UnityEngine;

namespace Uft.AdvTools.View
{
    [RequireComponent(typeof(RectTransform))]
    public class SelectionTitle : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected CanvasGroup? _canvasGroup; public CanvasGroup CanvasGroup => ThrowIf.Unassigned(this._canvasGroup);
        [SerializeField] protected TMP_Text? _txtText;

        // Methods

        public void SetData(string text, Vector2 anchoredPosition)
        {
            if (this._canvasGroup == null) throw new UnassignedReferenceException(nameof(this._canvasGroup));
            if (this._txtText == null) throw new UnassignedReferenceException(nameof(this._txtText));

            this._txtText.text = text;
            ((RectTransform)this.transform).anchoredPosition = anchoredPosition;
        }

        public virtual void Clear()
        {
            if (this._canvasGroup == null) throw new UnassignedReferenceException(nameof(this._canvasGroup));
            if (this._txtText == null) throw new UnassignedReferenceException(nameof(this._txtText));

            this._txtText.text = string.Empty;
            ((RectTransform)this.transform).anchoredPosition = Vector2.zero;
        }
    }
}
