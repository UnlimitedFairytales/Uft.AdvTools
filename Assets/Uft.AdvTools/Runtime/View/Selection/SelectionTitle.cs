#nullable enable

using TMPro;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class SelectionTitle : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected CanvasGroup? _canvasGroup; public CanvasGroup CanvasGroup => this._canvasGroup!;
        [SerializeField] protected TMP_Text? _txtText;

        // Methods

        public void SetData(string text, Vector2 ancharPosition)
        {
            if (this._txtText != null) this._txtText.text = text;
            ((RectTransform)this.transform).anchoredPosition = ancharPosition;
        }

        public virtual void Clear()
        {
            if (this._txtText != null) this._txtText.text = string.Empty;
        }
    }
}
